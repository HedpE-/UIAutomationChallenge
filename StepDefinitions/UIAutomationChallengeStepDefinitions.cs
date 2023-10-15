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
using TechTalk.SpecFlow;

namespace UiAutomationChallenge.StepDefinitions
{
    [Binding]
    public class UIAutomationChallengeStepDefinitions
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

        private string ChosenDevicePrice;
        private string ChosenProductName;
        private string Utm_Source;
        #endregion Private members

        #region Constructor
        public UIAutomationChallengeStepDefinitions()
        {
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            DriverBinaryPath = AssemblyName + "." + DriverBinaryPath + "." + DriverBinaryFileName;
        }
        #endregion Constructor

        #region Steps Definitions
        [When(@"I navigate to url ""([^""]*)""")]
        public void WhenINavigateToUrl(string url)
        {
            Driver.Url = url;
        }

        [Then(@"I expand the Purchase Price dropdown")]
        public void ThenIExpandThePurchsePriceDropdown()
        {
            var element = Driver.FindElement(By.Id("purchasePrice"));
            var shadowRoot = Driver.ExecuteJavaScript<IWebElement>("return(arguments[0].shadowRoot.querySelector('" + PurchasePrice_Combobox_Xpath + "'));", element);

            element = Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(PurchasePrice_Combobox_Xpath)));
            element.Click();

            element = Wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.XPath(PurchasePrice_DropdownPanel_Xpath)));
        }

        [Then(@"I click on device price ""([^""]*)""")]
        public void ThenIClickOnDevicePrice(string devicePrice)
        {
            ChosenDevicePrice = devicePrice;
        }

        [Then(@"I click Select on the Plan Card")]
        public void ThenIClickSelectOnThePlanCard()
        {
            Utm_Source = "";
            ChosenProductName = "";
        }

        [Then(@"I enter IMEI ""([^""]*)""")]
        public void ThenIEnterIMEI(string imei)
        {
            throw new PendingStepException();
        }

        [Then(@"I answer the Questionnaire with ""([^""]*)"" and ""([^""]*)"" and ""([^""]*)""")]
        public void ThenIAnswerTheQuestionnaireWithAndAnd(string ans1, string ans2, string ans3)
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the plan price")]
        public void ThenIValidateThePlanPrice()
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the Product Name")]
        public void ThenIValidateTheProductName()
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the Provider is ""([^""]*)""")]
        public void ThenIValidateTheProviderIs(string provider)
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the Contract Start date")]
        public void ThenIValidateTheContractStartDate()
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the Contract Renewal is ""([^""]*)""")]
        public void ThenIValidateTheContractRenewalIs(string period)
        {
            throw new PendingStepException();
        }

        [Then(@"I validate that there is no error shown")]
        public void ThenIValidateThatThereIsNoErrorShown()
        {
            throw new PendingStepException();
        }

        [Then(@"I validate that an error message is shown")]
        public void ThenIValidateThatAnErrorMessageIsShown()
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the page URL contains ""([^""]*)""")]
        public void ThenIValidateThePageURLContains(string text)
        {
            throw new PendingStepException();
        }

        [Then(@"I validate the utm_source was successfully carried from the previous page")]
        public void ThenIValidateTheUtm_SourceWasSuccessfullyCarriedFromThePreviousPage()
        {
            throw new PendingStepException();
        }


        [Then(@"I validate the combobox contains the chosen value")]
        public void ThenIValidateTheComboboxContainsTheChosenValue()
        {
            throw new PendingStepException();
        }

        [Given(@"I initialize the Selenium Driver with parameters")]
        public void GivenIInitializeTheSeleniumDriverWithParameters(Table table)
        {
            bool hideCommandPromptWindow = bool.Parse(table.Rows[0]["hideCommandPromptWindow"]);
            string proxy = table.Rows[0]["proxy"];
            bool headless = bool.Parse(table.Rows[0]["headless"]);
            string driverFilePath = table.Rows[0]["driverFilePath"];

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
                    if (e.Message.StartsWith("Access to the path") && e.Message.EndsWith("is denied."))
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
            if (headless)
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

        [Then(@"I stop the Selenium Driver")]
        public void ThenIStopSeleniumDriver()
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
        #endregion Steps Definitions

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
