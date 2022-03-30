namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private static string ErrorMessage = "Invalid Data";
        private static string SuccessDepartmentsMessage = "Imported {0} with {1} cells";
        private static string SuccessPrisonersMessage = "Imported {0} {1} years old";
        private static string SuccessOfficerMessage = "Imported {0} ({1} prisoners)";
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentDtos = JsonConvert.DeserializeObject<DepartmentDto[]>(jsonString);

            var departments = new List<Department>();

            var sb = new StringBuilder();

            foreach (var departmentDto in departmentDtos)
            {
                if (!IsValid(departmentDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name
                };

                bool isDepValid = true;
                foreach (var cellDto in departmentDto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        isDepValid = false;
                        break;
                    }

                    department.Cells.Add(new Cell()
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow
                    });
                }

                if (!isDepValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (department.Cells.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                departments.Add(department);
                sb.AppendLine(string.Format(SuccessDepartmentsMessage, department.Name, department.Cells.Count));
            }
            context.Departments.AddRange(departments);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            PrisonerDto[] prisonerDtos = JsonConvert.DeserializeObject<PrisonerDto[]>(jsonString);

            List<Prisoner> prisoners = new List<Prisoner>();

            foreach (PrisonerDto prisonerDto in prisonerDtos)
            {
                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime incarcerationDate;
                bool isIncarcerationDateValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out incarcerationDate);

                if (!isIncarcerationDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? releaseDate = null;
                if (!String.IsNullOrEmpty(prisonerDto.ReleaseDate))
                {
                    DateTime releaseDateValue;
                    bool isReleaseDateValid = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDateValue);

                    if (!isReleaseDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    releaseDate = releaseDateValue;
                }
                else
                {
                    releaseDate = null;
                }

                Prisoner p = new Prisoner()
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };

                bool areMailsValid = true;
                foreach (MailDto mailDto in prisonerDto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        areMailsValid = false;
                        continue;
                    }

                    p.Mails.Add(new Mail()
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    });
                }

                if (!areMailsValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                prisoners.Add(p);
                sb.AppendLine(String.Format(SuccessPrisonersMessage, p.FullName, p.Age));
            }

            context.Prisoners.AddRange(prisoners);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));

            List<Officer> officers = new List<Officer>();

            using (StringReader stringReader = new StringReader(xmlString))
            {
                OfficerDto[] officerDtos = (OfficerDto[])xmlSerializer.Deserialize(stringReader);

                foreach (OfficerDto officerDto in officerDtos)
                {
                    if (!IsValid(officerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    object positionObj;
                    bool isPositionValid = Enum.TryParse(typeof(Position), officerDto.Position, out positionObj);

                    if (!isPositionValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    object weaponObj;
                    bool isWeaponValid = Enum.TryParse(typeof(Weapon), officerDto.Weapon, out weaponObj);

                    if (!isWeaponValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Officer o = new Officer()
                    {
                        FullName = officerDto.FullName,
                        Salary = officerDto.Salary,
                        Position = (Position)positionObj,
                        Weapon = (Weapon)weaponObj,
                        DepartmentId = officerDto.DepartmentId
                    };

                    foreach (OfficerPrisonersDto prisonerDto in officerDto.OfficerPrisoners)
                    {
                        o.OfficerPrisoners.Add(new OfficerPrisoner()
                        {
                            Officer = o,
                            PrisonerId = prisonerDto.Id
                        });
                    }

                    officers.Add(o);
                    sb.AppendLine(String.Format(SuccessOfficerMessage, o.FullName, o.OfficerPrisoners.Count));
                }

                context.Officers.AddRange(officers);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}