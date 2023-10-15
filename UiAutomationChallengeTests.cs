using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace UiAutomationChallenge
{
    [TestFixture]
    public class UiAutomationChallengeTests
    {
        #region Private members
        private readonly string DriverBinaryFileName = "chromedriver.exe";
        private readonly string DriverBinaryPath = "Binaries";
        private readonly string AssemblyName;
        private readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(10);

        private readonly string PurchasePrice_Combobox_Xpath = "//input[@name='purchasePrice']";
        private readonly string PurchasePrice_DropdownPanel_Xpath = "//ul[@id='list']";

        private string DriverFilePath;
        private IWebDriver Driver;
        private WebDriverWait Wait;
        #endregion Private members

        #region Constructor
        public UiAutomationChallengeTests()
        {
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            DriverBinaryPath = AssemblyName + "." + DriverBinaryPath + "." + DriverBinaryFileName;
        }
        #endregion Constructor

        #region Test Cases
        [Test]
        public void TC01_FirstRequirement()
        {
            Driver.Url = "https://www.bolttech.co.th/en/ascend/device-protection?utm_source=ascend";

            var element = Driver.FindElement(By.Id("purchasePrice"));
            var shadowRoot = Driver.ExecuteJavaScript<IWebElement>("return(arguments[0].shadowRoot.querySelector('" + PurchasePrice_Combobox_Xpath + "'));", element);

            element = Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(PurchasePrice_Combobox_Xpath)));
            element.Click();

            element = Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(PurchasePrice_DropdownPanel_Xpath)));
        }

        [Test]
        public void TC02_SecondRequirement()
        {
        }

        [Test]
        public void TC03_ThirdRequirement()
        {
        }

        [Test]
        public void TC05_FifthRequirement()
        {
        }

        [Test]
        public void TC06_SixthRequirement()
        {
        }

        [Test]
        public void TC07_SeventhRequirement()
        {
        }

        [Test]
        public void TC08_EighthRequirement()
        {
        }
        #endregion Test Cases

        #region Setup & TearDown methods
        [SetUp]
        public void Setup()
        {
            InitiateChromeDriver(true, false);
        }

        [TearDown]
        public void TearDown()
        {
            var processes = Process.GetProcessesByName(DriverBinaryFileName.Replace(".exe", ""));
            if (processes.Any())
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                    catch { }
                }
            }

            if (File.Exists(DriverFilePath))
                File.Delete(DriverFilePath);
        }
        #endregion Setup & TearDown methods

        #region Private methods
        private string ExtractResourceToLocalFile(string resourceName, string assemblyName, string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
                filePath = Path.GetTempFileName();

            var assembly = GetAssemblyByName(assemblyName);

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (MemoryStream reader = new MemoryStream())
            {
                stream.CopyTo(reader);
                File.WriteAllBytes(filePath, reader.ToArray());
            }

            return filePath;
        }

        private Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
        }

        private void InitiateChromeDriver(bool hideCommandPromptWindow, bool headless, string driverFilePath = null)
        {
            if (!string.IsNullOrEmpty(driverFilePath))
            {
                if (!IsValidPath(driverFilePath))
                    throw new ArgumentException($"driverFilePath value '{driverFilePath}' is invalid.");

                if (string.IsNullOrEmpty(Path.GetFileName(driverFilePath)))
                    driverFilePath = Path.Combine(driverFilePath, DriverBinaryFileName);
                else
                {
                    FileInfo file = new FileInfo(driverFilePath);
                    if (file.Extension.ToLower() != ".exe")
                        throw new ArgumentException($"driverFilePath value '{driverFilePath}' is invalid. File extension '{file.Extension}' is not an executable type.");
                }
            }
            else
                driverFilePath = Path.Combine(Path.GetTempPath(), DriverBinaryFileName);

            if (File.Exists(driverFilePath))
            {
                try
                {
                    File.Delete(driverFilePath);
                }
                catch (Exception e)
                {
                    if (e.Message.StartsWith("Access to the path ") && e.Message.EndsWith("' is denied."))
                    {
                        var allProcesses = Process.GetProcesses().Where(p =>
                        {
                            try { return p.MainModule.FileName == driverFilePath; } catch { return false; }
                        });
                        foreach (Process proc in allProcesses)
                        {
                            try
                            {
                                proc.Kill();
                                Thread.Sleep(2000);
                                File.Delete(driverFilePath);
                            }
                            catch (Exception e2)
                            {
                                throw e2;
                            }
                        }
                    }
                }
            }

            ExtractResourceToLocalFile(DriverBinaryPath, AssemblyName, driverFilePath);

            ChromeOptions options = new ChromeOptions();

            options.AddArgument("--user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1");
                options.AddArgument("ignore-certificate-errors");
            if(headless)
                options.AddArgument("--headless");
            if (hideCommandPromptWindow)
            {
                var driverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(driverFilePath), Path.GetFileName(driverFilePath));
                driverService.HideCommandPromptWindow = hideCommandPromptWindow;
                Driver = new ChromeDriver(driverService, options);
            }
            else
                Driver = new ChromeDriver(options);

            Wait = new WebDriverWait(Driver, WaitTimeout);

            Driver.Manage().Window.Maximize();
        }

        private bool IsValidPath(string pathString)
        {
            if (string.IsNullOrWhiteSpace(pathString) || pathString.Length < 3)
                return false;

            Regex driveCheck = new Regex(@"^[a-zA-Z]:\\$");
            if (!driveCheck.IsMatch(pathString.Substring(0, 3)))
                return false;

            string strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
            strTheseAreInvalidFileNameChars += @":/?*" + "\"";
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
            if (containsABadCharacter.IsMatch(pathString.Substring(3, pathString.Length - 3)))
                return false;

            string tempPath = Path.GetFullPath(pathString);
            if (!string.IsNullOrEmpty(Path.GetFileName(pathString)))
                tempPath = tempPath.Replace(Path.GetFileName(pathString), "");
            DirectoryInfo directoryInfo = new DirectoryInfo(tempPath);
            if (!directoryInfo.Exists)
                return false;

            return true;
        }
        #endregion Private methods
    }
}
