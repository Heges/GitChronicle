using Changeloger.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ChangelogParser.Models
{
    public class CreatePDFDocument
    {
        private IConfiguration _configuration;
        private IOptions<PDFOptions> _pdfoptions;

        public CreatePDFDocument(IServiceProvider provider)
        {
            _configuration = provider.GetRequiredService<IConfiguration>();
            _pdfoptions = provider.GetRequiredService<IOptions<PDFOptions>>();
        }

        public string Create(string rootPath, Dictionary<string, Changelog> dict, string platformName)
        {
            var absolutePath = Path.Combine(Path.Combine(rootPath, platformName), "Changelogs");
            QuestPDF.Settings.License = LicenseType.Community;
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(_pdfoptions.Value.HeaderMarginSize, Unit.Centimetre);

                    page.Header()
                        .Text(platformName)
                        .SemiBold()
                        .FontSize(_pdfoptions.Value.HeaderFontSize)
                        .AlignCenter();

                    page.Content()
                        .PaddingTop(_pdfoptions.Value.ContentBodyPaddingTopSize)
                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                        .PaddingRight(_pdfoptions.Value.ContentPaddingRightSize)
                        .Column(column =>
                        {
                            column.Spacing(_pdfoptions.Value.HeaderSpacingSize);

                            foreach (var kvp in dict)
                            {
                                column.Item()
                                    .PaddingBottom(_pdfoptions.Value.ContentBodyPaddingBottomSize)
                                    .Text(kvp.Key)
                                    .SemiBold()
                                    .FontSize(_pdfoptions.Value.ContentHeaderFontSize)
                                    .Underline();

                                foreach (var value in kvp.Value.Items)
                                {
                                    column.Item()
                                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                                        .Text(text =>
                                        {
                                            text.Span($"Заголовок: ")
                                            .Bold()
                                            .FontSize(_pdfoptions.Value.ContentBodyFontSize);
                                            text.Span(value.ChangelogItemTitle)
                                            .FontSize(_pdfoptions.Value.ContentBodyFontSize);
                                        });
                                    column.Item()
                                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                                        .Text(text =>
                                        {
                                            text.Span($"Автор: ")
                                            .Bold()
                                            .FontSize(_pdfoptions.Value.ContentBodyFontSize);
                                            text.Span(value.ChangelogItemAuthor)
                                            .FontSize(_pdfoptions.Value.ContentBodyFontSize);
                                        });
                                    column.Item()
                                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                                        .Text(text =>
                                        {
                                            text.Span($"Дата: ")
                                            .Bold()
                                            .FontSize(_pdfoptions.Value.ContentBodyPaddingLeftSize);
                                            text.Span(value.ChangelogItemDate)
                                            .FontSize(_pdfoptions.Value.ContentBodyPaddingLeftSize);
                                        });
                                    column.Item()
                                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                                        .Text(text =>
                                        {
                                            text.Span($"Описание: ")
                                            .Bold()
                                            .FontSize(_pdfoptions.Value.ContentBodyPaddingLeftSize);
                                            text.Span(value.ChangelogItemDescription)
                                            .FontSize(_pdfoptions.Value.ContentBodyPaddingLeftSize);
                                        });
                                    column.Item()
                                        .PaddingLeft(_pdfoptions.Value.ContentBodyPaddingLeftSize)
                                        .PaddingBottom(_pdfoptions.Value.ContentBodyPaddingBottomSize)
                                        .Text(text =>
                                        {
                                            text.Span($"")
                                            .FontSize(_pdfoptions.Value.ContentBodyFontSize);
                                        });
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.Span("Страница ").FontSize(_pdfoptions.Value.ContentFooterFontSize);
                            txt.CurrentPageNumber();
                            txt.Span(" из ").FontSize(_pdfoptions.Value.ContentFooterFontSize);
                            txt.TotalPages();
                        });
                });
            });

            if(!Directory.Exists(absolutePath))
                Directory.CreateDirectory(absolutePath);

            string outputFile = Path.Combine(absolutePath, $"changelog-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm")}");
            document.GeneratePdf($"{outputFile}.pdf");
            Console.WriteLine($"Документ сформирован {outputFile}.pdf");
            return outputFile;
        }
    }
}
