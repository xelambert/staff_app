using System.Text.RegularExpressions;

namespace test_staff
{
    public static class StaffValidator
    {
        const int MIN_AGE = 18;
        const int MAX_AGE = 100;
        static public bool validate(Staff staff, out string validationMessage)
        {
            validationMessage = "";

            bool isValidated = true;

            if (!validatePIB(staff.PIB))
            {
                validationMessage += " Невірно заповнено ПІБ. \n";
                isValidated = false;
            }

            if (!validateSalary(staff.salary))
            {
                validationMessage += " Невірно заповнено зарплату. \n";
                isValidated = false;
            }

            if(!validateBirthDate(staff.birthDate))
            {
                validationMessage += " Невірно заповнено День Народження. \n";
                isValidated = false;
            }

            return isValidated;
        }

        public static bool validatePIB(string? PIB)
        {
            bool isPIBValidated = true;

            if (String.IsNullOrEmpty(PIB) ||
                PIB.Length > 100 ||
                !Regex.IsMatch(PIB, @"^[a-zA-Zа-яА-ЯіІїЇєЄ'\s]+$"))
            {
                isPIBValidated = false;
            }

            return isPIBValidated;
        }

        public static bool validateSalary(float? salary)
        {
            bool isSalaryValidated = true;

            if (!salary.HasValue ||
                salary.Value <= 0)
            {
                isSalaryValidated = false;
            }

            return isSalaryValidated;
        }

        public static bool validateBirthDate(DateTime? birthDate)
        {
            bool isBirthDateValidated = true;

            if(!birthDate.HasValue ||
                calculateAge(birthDate.Value) < MIN_AGE ||
                calculateAge(birthDate.Value) > MAX_AGE)
            {
                isBirthDateValidated = false;
            }

            return isBirthDateValidated;
        }

        static int calculateAge(DateTime birthDate)
        {
            DateTime currentdate = DateTime.Now;

            int years = currentdate.Year - birthDate.Year;

            if (currentdate < birthDate.AddYears(years))
            {
                years--;
            }

            return years;
        }
    }
}
