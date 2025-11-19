using AcademicClaimsPrototype.Filters;
using AcademicClaimsPrototype.Models;
using AcademicClaimsPrototype.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AcademicClaimsPrototype.Controllers
{
    [AuthorizeRole("HR")]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HRController(ApplicationDbContext context)
        {
            _context = context;
            // Set QuestPDF license (free for open source and development)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<IActionResult> Index()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved)
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            var allUsers = await _context.Users.ToListAsync();

            ViewBag.ApprovedClaims = approvedClaims;
            ViewBag.Users = allUsers;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePaymentReport()
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved)
                .OrderBy(c => c.LecturerEmail)
                .ThenBy(c => c.Date)
                .ToListAsync();

            var grouped = approvedClaims.GroupBy(c => c.LecturerEmail);

            var generatedInvoicePaths = new List<string>();
            foreach (var g in grouped)
            {
                var claimsForLecturer = g.ToList();
                try
                {
                    // Generate PDF using QuestPDF
                    var invoiceModel = new InvoiceViewModel
                    {
                        LecturerEmail = g.Key,
                        GeneratedDate = DateTime.Now,
                        Claims = claimsForLecturer
                    };

                    // Save PDF to file
                    var invoicesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices", SanitizeForPath(g.Key));
                    if (!Directory.Exists(invoicesDir))
                        Directory.CreateDirectory(invoicesDir);

                    var fileName = $"invoice-{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    var fullPath = Path.Combine(invoicesDir, fileName);

                    // Generate PDF using QuestPDF
                    var pdfBytes = GeneratePdfInvoice(invoiceModel);
                    await System.IO.File.WriteAllBytesAsync(fullPath, pdfBytes);

                    var relativePath = $"/invoices/{SanitizeForPath(g.Key)}/{fileName}";
                    generatedInvoicePaths.Add(relativePath);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error generating invoice for {g.Key}: {ex.Message}";
                }
            }

            var totalAmount = approvedClaims.Sum(c => c.Amount);
            var claimCount = approvedClaims.Count;
            var invoicesCount = generatedInvoicePaths.Count;

            TempData["Success"] = $"Payment report generated! {claimCount} approved claims totalling R {totalAmount:N2}. {invoicesCount} PDF invoices created.";
            TempData["GeneratedInvoices"] = string.Join(";", generatedInvoicePaths);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DownloadInvoice(string lecturerEmail)
        {
            var approvedClaims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Approved && c.LecturerEmail == lecturerEmail)
                .OrderBy(c => c.Date)
                .ToListAsync();

            var invoiceModel = new InvoiceViewModel
            {
                LecturerEmail = lecturerEmail,
                GeneratedDate = DateTime.Now,
                Claims = approvedClaims
            };

            // Generate PDF using QuestPDF
            var pdfBytes = GeneratePdfInvoice(invoiceModel);
            return File(pdfBytes, "application/pdf", $"invoice-{lecturerEmail}-{DateTime.Now:yyyyMMdd}.pdf");
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string claimId, string paymentReference)
        {
            if (string.IsNullOrEmpty(claimId))
            {
                TempData["Error"] = "Invalid claim ID.";
                return RedirectToAction("Index");
            }

            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == claimId);
            if (claim == null)
            {
                TempData["Error"] = "Claim not found.";
                return RedirectToAction("Index");
            }

            if (claim.Status != ClaimStatus.Approved)
            {
                TempData["Error"] = "Only approved claims can be processed for payment.";
                return RedirectToAction("Index");
            }

            if (claim.IsPaid)
            {
                TempData["Error"] = "This claim has already been paid.";
                return RedirectToAction("Index");
            }

            try
            {
                claim.IsPaid = true;
                claim.PaidDate = DateTime.UtcNow;
                claim.PaidBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail) ?? "HR User";
                claim.PaymentReference = string.IsNullOrWhiteSpace(paymentReference)
                    ? $"PAY-{DateTime.Now:yyyyMMddHHmmss}"
                    : paymentReference;

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Payment processed successfully for claim {claim.Id}. Reference: {claim.PaymentReference}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error processing payment: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BulkProcessPayments(string[] claimIds, string bulkPaymentReference)
        {
            if (claimIds == null || claimIds.Length == 0)
            {
                TempData["Error"] = "No claims selected for payment processing.";
                return RedirectToAction("Index");
            }

            var processedCount = 0;
            var errors = new List<string>();

            foreach (var claimId in claimIds)
            {
                var claim = await _context.Claims.FirstOrDefaultAsync(c => c.Id == claimId);
                if (claim != null && claim.Status == ClaimStatus.Approved && !claim.IsPaid)
                {
                    try
                    {
                        claim.IsPaid = true;
                        claim.PaidDate = DateTime.UtcNow;
                        claim.PaidBy = HttpContext.Session.GetString(AuthorizeRoleAttribute.SessionEmail) ?? "HR User";
                        claim.PaymentReference = string.IsNullOrWhiteSpace(bulkPaymentReference)
                            ? $"BULK-PAY-{DateTime.Now:yyyyMMddHHmmss}-{processedCount + 1}"
                            : $"{bulkPaymentReference}-{processedCount + 1}";

                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Claim {claim.Id}: {ex.Message}");
                    }
                }
            }

            if (processedCount > 0)
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Successfully processed payments for {processedCount} claims.";
            }

            if (errors.Count > 0)
            {
                TempData["Error"] = $"Some errors occurred: {string.Join("; ", errors)}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLecturerInfo(string email, string newEmail, string fullName, string phoneNumber, string department)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            // Store the original email for comparison
            var originalEmail = user.Email;

            if (!string.IsNullOrEmpty(newEmail) && !newEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == newEmail);
                if (existingUser != null)
                {
                    TempData["Error"] = "Email already exists.";
                    return RedirectToAction("Index");
                }

                // Special handling for email change (primary key)
                // Create a new user with the new email and delete the old one
                var newUser = new User
                {
                    Email = newEmail,
                    Password = user.Password,
                    Role = user.Role,
                    FullName = !string.IsNullOrEmpty(fullName) ? fullName : user.FullName,
                    PhoneNumber = !string.IsNullOrEmpty(phoneNumber) ? phoneNumber : user.PhoneNumber,
                    Department = !string.IsNullOrEmpty(department) ? department : user.Department
                };

                // Update all claims associated with the old email
                var userClaims = await _context.Claims
                    .Where(c => c.LecturerEmail == originalEmail)
                    .ToListAsync();

                foreach (var claim in userClaims)
                {
                    claim.LecturerEmail = newEmail;
                }

                // Add new user and remove old user
                _context.Users.Add(newUser);
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Lecturer information updated successfully! Email changed from " + originalEmail + " to " + newEmail;
                return RedirectToAction("Index");
            }
            else
            {
                // No email change, just update other fields
                if (!string.IsNullOrEmpty(fullName))
                    user.FullName = fullName;

                if (!string.IsNullOrEmpty(phoneNumber))
                    user.PhoneNumber = phoneNumber;

                if (!string.IsNullOrEmpty(department))
                    user.Department = department;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Lecturer information updated successfully!";
                return RedirectToAction("Index");
            }
        }

        public IActionResult ViewDocument(string documentPath)
        {
            if (string.IsNullOrEmpty(documentPath))
            {
                return NotFound();
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", documentPath.TrimStart('/'));
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            var contentType = GetContentType(fullPath);
            return File(fileBytes, contentType, Path.GetFileName(fullPath));
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
            {
                { ".pdf", "application/pdf" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".jpg", "image/jpeg" },
                { ".png", "image/png" }
            };

            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

        private string SanitizeForPath(string input)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '-');
            }
            return input.Replace("@", "_at_");
        }

        // PDF Generation Method using QuestPDF
        private byte[] GeneratePdfInvoice(InvoiceViewModel model)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text("UNIVERSITY CLAIMS INVOICE")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken3);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Invoice Details
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(150);
                                    columns.RelativeColumn();
                                });

                                table.Cell().Text("Lecturer:").SemiBold();
                                table.Cell().Text(model.LecturerEmail);

                                table.Cell().Text("Generated Date:").SemiBold();
                                table.Cell().Text(model.GeneratedDate.ToString("yyyy-MM-dd HH:mm"));

                                table.Cell().Text("Number of Claims:").SemiBold();
                                table.Cell().Text(model.Claims.Count.ToString());
                            });

                            // Claims Table
                            column.Item().Text("CLAIMS DETAILS").SemiBold().FontSize(14);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Description
                                    columns.ConstantColumn(80); // Date
                                    columns.ConstantColumn(60); // Hours
                                    columns.ConstantColumn(80); // Rate
                                    columns.ConstantColumn(80); // Amount
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").SemiBold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Date").SemiBold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Hours").SemiBold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Rate (R)").SemiBold();
                                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Amount (R)").SemiBold();
                                });

                                // Rows
                                foreach (var claim in model.Claims)
                                {
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(claim.Description);
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(claim.Date.ToString("yyyy-MM-dd"));
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(claim.Hours.ToString());
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(claim.Rate.ToString("N2"));
                                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(claim.Amount.ToString("N2"));
                                }
                            });

                            // Total Amount
                            column.Item().AlignRight().Text($"Total Amount: R {model.TotalAmount:N2}")
                                .SemiBold().FontSize(16).FontColor(Colors.Green.Darken3);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated by Academic Claims System - ");
                            x.Span(DateTime.Now.Year.ToString());
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}