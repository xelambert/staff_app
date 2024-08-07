namespace test_staff.Testing
{
    public class StaffValidatorTests
    {
        [TestFixture]
        public class StaffValidatorTest
        {

            [SetUp]
            public void Setup()
            {
                ;
            }

            [Test]
            public void validatePIB_digitValues_shouldReturnFalse()
            {
                bool result = StaffValidator.validatePIB("Грищенко Тим0фій Юрійович");
                Assert.AreEqual(false, result);
            }

            [Test]
            public void validatePIB_normalValue_shouldReturnTrue()
            {
                bool result = StaffValidator.validatePIB("Пір'їнков Єгор Валентинович");
                Assert.AreEqual(true, result);
            }

            [Test]
            public void validateSalary_zeroValue_shouldReturnFalse()
            {
                bool result = StaffValidator.validateSalary(0);
                Assert.AreEqual(false, result);
            }

            [Test]
            public void validateSalary_negativeValue_shouldReturnFalse()
            {
                bool result = StaffValidator.validateSalary(-50);
                Assert.AreEqual(false, result);
            }

            [Test]
            public void validateSalary_normalValue_shouldReturnTrue()
            {
                bool result = StaffValidator.validateSalary(50);
                Assert.AreEqual(true, result);
            }

            [Test]
            public void validateBirthDate_now_shouldReturnFalse()
            {
                bool result = StaffValidator.validateBirthDate(DateTime.Now);
                Assert.AreEqual(false, result);
            }

            [Test]
            public void validateBirthDate_nowMinus20Years_shouldReturnTrue()
            {
                bool result = StaffValidator.validateBirthDate(DateTime.Now.AddYears(-20));
                Assert.AreEqual(true, result);
            }

            [Test]
            public void validateBirthDate_nowMinus150Years_shouldReturnFalse()
            {
                bool result = StaffValidator.validateBirthDate(DateTime.Now.AddYears(-150));
                Assert.AreEqual(false, result);
            }
        }
    }
}