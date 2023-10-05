using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace SSCElectionAutomation
{
    public class Tests
    {
        IWebDriver driver;
        [SetUp]
        public void Setup()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddArgument("--ignore-certificate-errors");

            string path = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;

            driver = new ChromeDriver(path + @"\Drivers\", options);
            driver.Manage().Window.Maximize();
        }


        [Test]
        public void T1_DELETE()
        {
           
            driver.Navigate().GoToUrl("http://124.105.188.119/rc_voting/admin/voters.php");
            Thread.Sleep(1000);

            driver.FindElement(By.XPath("//input[@type='text']")).Click();
            driver.FindElement(By.XPath("//input[@type='text']")).SendKeys("Rogationistcollege");
            driver.FindElement(By.XPath("//input[@type='password']")).Click();
            driver.FindElement(By.XPath("//input[@type='password']")).SendKeys("rc2022");
            driver.FindElement(By.XPath("//button[@type='submit']")).Click();
            
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//span[text()='Voters']")).Click();
            Thread.Sleep(3000);
            


            string xpathToWaitFor = "//tbody//tr[1]//button[@class='btn btn-danger btn-sm delete btn-flat']";

            while (IsElementPresent(driver, By.XPath(xpathToWaitFor)))
            {
                
                driver.FindElement(By.XPath(xpathToWaitFor)).Click();
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//button[@name='delete']")).Click();
            }

            driver.Quit();
        }
        static bool IsElementPresent(IWebDriver driver, By by)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(by);
            return elements.Count > 0; // Return true if at least one element is found
        }
        [Test]
        public void T2_ADD()
        {
            driver.Navigate().GoToUrl("http://124.105.188.119/rc_voting/admin/index.php");

            string jsonFilePath = @"C:\Users\nkkds\source\repos\SSCElectionAutomation\SSCElectionAutomation\Files\userdata.json";
            List<UserData> users = JsonConvert.DeserializeObject<List<UserData>>(File.ReadAllText(jsonFilePath));
            driver.FindElement(By.XPath("//input[@name='username']")).Click();
            driver.FindElement(By.XPath("//input[@name='username']")).SendKeys("Rogationistcollege");
            driver.FindElement(By.XPath("//input[@name='password']")).Click();
            driver.FindElement(By.XPath("//input[@name='password']")).SendKeys("rc2022");
            driver.FindElement(By.XPath("//button[@name='login']")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//a[@href='voters.php']")).Click();


            foreach (UserData user in users)
            {

                Thread.Sleep(500);
                driver.FindElement(By.XPath("//a[@href='#addnew']")).Click();
                Thread.Sleep(500);
                driver.FindElement(By.XPath("(//input[@id='firstname'])[2]")).Click();
                driver.FindElement(By.XPath("(//input[@id='firstname'])[2]")).SendKeys(user.firstName);

                driver.FindElement(By.XPath("(//input[@id='lastname'])[2]")).Click();
                driver.FindElement(By.XPath("(//input[@id='lastname'])[2]")).SendKeys(user.lastName);

                driver.FindElement(By.XPath("(//input[@id='password'])[2]")).Click();
                driver.FindElement(By.XPath("(//input[@id='password'])[2]")).SendKeys(user.passWord);

                driver.FindElement(By.XPath("//button[@name='add']")).Click();
            }

            driver.Quit();
        }
        [Test]
        public void T3_FETCH()
        {
            driver.Navigate().GoToUrl("http://124.105.188.119/rc_voting/admin/index.php");

            string jsonFilePath = @"C:\Users\nkkds\source\repos\SSCElectionAutomation\SSCElectionAutomation\Files\userdata.json";
            List<UserData> voters = JsonConvert.DeserializeObject<List<UserData>>(File.ReadAllText(jsonFilePath));
            driver.FindElement(By.XPath("//input[@name='username']")).Click();
            driver.FindElement(By.XPath("//input[@name='username']")).SendKeys("Rogationistcollege");
            driver.FindElement(By.XPath("//input[@name='password']")).Click();
            driver.FindElement(By.XPath("//input[@name='password']")).SendKeys("rc2022");
            driver.FindElement(By.XPath("//button[@name='login']")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.XPath("//a[@href='voters.php']")).Click();

            foreach (UserData voter in voters)
            {
                // Concatenate firstName and lastName with a space in between
                string fullName = $"{voter.firstName} {voter.lastName}";

                IWebElement searchInput = driver.FindElement(By.XPath("//input[@type='search']"));
                searchInput.Clear();
                searchInput.SendKeys(fullName);
                Thread.Sleep(1000);

                IList<IWebElement> matchingElements = driver.FindElements(By.XPath("//tbody//tr//td[4]"));

                if (matchingElements.Count == 1)
                {
                    string voterIdText = matchingElements[0].Text;
                    voter.voterId = voterIdText;
                }
                else if (matchingElements.Count > 1)
                {
                    IList<IWebElement> firstNameElements = driver.FindElements(By.XPath("//tbody//tr//td[4]/preceding-sibling::td[2]"));

                    for (int i = 0; i < matchingElements.Count; i++)
                    {
                        string firstNameText = firstNameElements[i].Text;
                        if (firstNameText == fullName)
                        {
                            string voterIdText = matchingElements[i].Text;
                            voter.voterId = voterIdText;
                            break;
                        }
                    }
                }

                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(voters, Formatting.Indented));
            }

            driver.Quit();
        }
    }
    public class UserData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string passWord { get; set; }
        public string voterId { get; set; }
    }
}