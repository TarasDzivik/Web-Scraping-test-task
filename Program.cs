using HtmlAgilityPack;
using System.Reflection;
using WebScraping.Services;

class Program
{
    static async System.Threading.Tasks.Task Main(string[] args)
    {
        try
        {
            #region Fields_Setting
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string address = "https://www.abs.gov.au/statistics/labour/employment-and-unemployment/labour-force-australia";

            // Debug mode (this path works correct only in debug)
            string path = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.Parent!.Parent!.Parent!.FullName, "Dowloads");

            // Release mode
            //string path = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Downloads");

            string fileName = "data.xlsl";

            var baseaddress = AddressHelper.GetBaseAddres(address);
            Console.WriteLine($"Website address is: {baseaddress}");

            var api = AddressHelper.GetApi(address);
            Console.WriteLine($"First-page api: {api}");

            var firstPage = new HtmlDocument();
            var secondPage = new HtmlDocument();
            #endregion

            #region First_Step
            string firstPageaddress = AddressHelper.MargeHostAndApi(baseaddress, api);
            Console.WriteLine($"The first-page address is: {firstPageaddress}");
            if (await HtmlDoksExtractor.CheckResponseSuccess(firstPageaddress))
            {
                firstPage = HtmlDoksExtractor.GetHtmlDocument(firstPageaddress);
                Console.WriteLine($"The first page is loaded.");
            }

            string LatestReleaseBlock = "//span[contains(@class, 'flag_latest')]/preceding-sibling::a";
            string? LatestReleaseUrl = HtmlDoksExtractor.FindApi(firstPage, LatestReleaseBlock);

            if (string.IsNullOrEmpty(LatestReleaseUrl))
            {
                throw new NullReferenceException($"Download Url is null or empty: {LatestReleaseUrl}");
            }
            string secondPageaddress = AddressHelper.MargeHostAndApi(baseaddress, LatestReleaseUrl);
            Console.WriteLine($"The second page ardess is: {secondPageaddress}");
            #endregion

            #region Second_Step
            string downloadFirstTableBlock = "//div[@class='file-description-link-formatter']//a[@download]";
            if (await HtmlDoksExtractor.CheckResponseSuccess(secondPageaddress))
            {
                secondPage = HtmlDoksExtractor.GetHtmlDocument(secondPageaddress);
                Console.WriteLine($"The second page is loaded!");
            }

            var downloadUrl = HtmlDoksExtractor.FindApi(secondPage, downloadFirstTableBlock);

            Console.WriteLine($"The download url is found:\nDowload file url: {downloadUrl}");

            if (string.IsNullOrEmpty(downloadUrl))
            {
                throw new NullReferenceException($"Download Url is null or empty: {downloadUrl}");
            }

            string downloadaddress = AddressHelper.MargeHostAndApi(baseaddress, downloadUrl);
            #endregion

            #region Third_Step
            bool isFileDownloaded = FileDowloader.DowloadFileFromWeb(downloadaddress, path, fileName);

            if (!isFileDownloaded)
            {
                throw new Exception($"\nProblem with downloading file:\n\'{fileName}\'\ninto the folder: {path}");
            }
            #endregion

            #region Fourth_Step
            var dataSet = FileEditor.GetDataSetFromFile(path, fileName);
            if (dataSet is null)
            {
                throw new NullReferenceException($"\nDataSet is null or empty: {downloadUrl}");
            }
            #endregion

            #region Fifth_Step

            var newDataTeble = FileEditor.TransposeDataTable(dataSet);
            if (newDataTeble is null || newDataTeble.Columns.Count is 0)
            {
                throw new NullReferenceException($"Problem with Transposing Data. newDataTable is null or empty: {newDataTeble}");
            }
            #endregion

            #region Final_Step
            FileEditor.SaveAsCsvFile(path, "result.csv", newDataTeble);
            Console.WriteLine();
            #endregion

            Console.ReadKey();
        }
        catch (NullReferenceException nullEx)
        {
            Console.WriteLine(nullEx.Message);
            Console.WriteLine(nullEx.StackTrace);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
