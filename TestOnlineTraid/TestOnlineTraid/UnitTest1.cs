using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomatedTests
{
    public class Tests
    {
        IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl("https://www.wildberries.ru");

        }

        [Test]
        public void TestPriceFilter()
        {
            driver.FindElement(By.ClassName("search-catalog__input")).Click();
            driver.FindElement(By.ClassName("search-catalog__input")).SendKeys("Iphone 11");
            driver.FindElement(By.Id("applySearchBtn")).Click();

            Thread.Sleep(1000);

            var from = Convert.ToInt32(driver.FindElement(By.CssSelector(".catalog-page .filter .selectorsblock .range-text .start-n input")).GetAttribute("value"));

            driver.FindElement(By.CssSelector(".catalog-page .filter .selectorsblock .range-text .end-n input")).Clear();

            var to = Convert.ToInt32(driver.FindElement(By.CssSelector(".catalog-page .filter .selectorsblock .range-text .end-n input")).GetAttribute("value"));

            string[] values = driver.FindElements(By.ClassName("lower-price")).Select(webPrice => webPrice.Text).ToArray<string>();

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = String.Join("", values[i].Where(c => char.IsDigit(c)));
            }

            int[] actualValues = Array.ConvertAll(values, s => int.Parse(s));
            actualValues.ToList().ForEach(actualPrice => Assert.True(actualPrice >= from && actualPrice <= to, "Price filter works wrong. Actual price is " + actualPrice + ". But should be more or equal than 39 and less or equal than 239"));
        }

        [Test]
        public void NegativeSignUpTest()
        {

            driver.FindElement(By.CssSelector("[href*='/security/login?returnUrl=https%3A%2F%2Fwww.wildberries.ru%2F']")).Click();
            driver.FindElement(By.ClassName("input-item")).SendKeys("1111111111");
            driver.FindElement(By.Id("requestCode")).Click();
            driver.FindElement(By.Id("smsCaptchaCode")).Click();
            driver.FindElement(By.Id("smsCaptchaCode")).SendKeys("12345");
            driver.FindElement(By.CssSelector(".btn-main-lg")).Click();
            Assert.IsTrue(driver.FindElements(By.CssSelector(".form-block .field-validation-error, .form-block .form-block__message--error")).Any(), "Код указан неверно");

        }

        [Test]
        public void TestTooltipText()
        {

            new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[href*='/services/besplatnaya-dostavka?desktop=1#terms-delivery']"))).Build().Perform();

            new WebDriverWait(driver, TimeSpan.FromSeconds(2))
            .Until(x => driver.FindElement(By.CssSelector(".tooltip-addresses .tooltipster-content")).Text != string.Empty);

            /* Данная проверка не корректна, потому что данные по пунктам в Wildberries меняются рандомно каждые 10-15 сек
            Assert.AreEqual("18 980 пунктов выдачи\r\n731 постамат", driver.FindElement(By.CssSelector(".tooltip-addresses .tooltipster-content")).Text.Trim(),
            "Tooltip has not appeared or Text is inCorected."); */
        }
    }
}