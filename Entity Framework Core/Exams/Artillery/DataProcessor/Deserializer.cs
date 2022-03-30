namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.Data.Models.Enums;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSearializer = new XmlSerializer(typeof(ImportCountryDto[]), new XmlRootAttribute("Countries"));

            var countries = new List<Country>();

            using StringReader stringReader = new StringReader(xmlString);

            var countriesDtos = (ImportCountryDto[])xmlSearializer.Deserialize(stringReader);

            if (countriesDtos.Any())
            {
                foreach (var countryDto in countriesDtos)
                {
                    if (!IsValid(countryDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool alreadyExists = countries.Any(c => c.CountryName == countryDto.CountryName
                                                            && c.ArmySize == countryDto.ArmySize);
                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var country = new Country()
                    {
                        CountryName = countryDto.CountryName,
                        ArmySize = countryDto.ArmySize
                    };

                    countries.Add(country);
                    sb.AppendLine(string.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
                }
            }
            else
            {
                sb.AppendLine(ErrorMessage);
                return sb.ToString().TrimEnd();
            }

            context.Countries.AddRange(countries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSearializer = new XmlSerializer(typeof(ImportManufacturerDto[]), new XmlRootAttribute("Manufacturers"));

            var manufacturers = new List<Manufacturer>();

            using StringReader stringReader = new StringReader(xmlString);

            var manufacturerDtos = (ImportManufacturerDto[])xmlSearializer.Deserialize(stringReader);

            if (manufacturerDtos.Any())
            {
                foreach (var manufacturerDto in manufacturerDtos)
                {
                    if (!IsValid(manufacturerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var foundedInfo = manufacturerDto.Founded.Split(", ").ToArray();

                    bool alreadyExists = manufacturers.Any(m => m.ManufacturerName == manufacturerDto.ManufacturerName
                                                            || m.Founded == manufacturerDto.Founded);

                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var manufacturer = new Manufacturer()
                    {
                        ManufacturerName = manufacturerDto.ManufacturerName,
                        Founded = manufacturerDto.Founded
                    };

                    manufacturers.Add(manufacturer);
                    var townName = foundedInfo[foundedInfo.Length - 2];
                    var countryName = foundedInfo[foundedInfo.Length - 1];
                    sb.AppendLine(string.Format(SuccessfulImportManufacturer, manufacturer.ManufacturerName,
                                                                                townName + ", " + countryName));
                }
            }
            else
            {
                sb.AppendLine(ErrorMessage);
                return sb.ToString().TrimEnd();
            }

            context.Manufacturers.AddRange(manufacturers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xmlSearializer = new XmlSerializer(typeof(ImportShellDto[]), new XmlRootAttribute("Shells"));

            var shells = new List<Shell>();

            using StringReader stringReader = new StringReader(xmlString);

            var shellsDtos = (ImportShellDto[])xmlSearializer.Deserialize(stringReader);

            if (shellsDtos.Any())
            {
                foreach (var shellDto in shellsDtos)
                {
                    if (!IsValid(shellDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool alreadyExists = shells.Any(s => s.ShellWeight == shellDto.ShellWeight
                                                            && s.Caliber == shellDto.Caliber);
                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var shell = new Shell()
                    {
                        ShellWeight = shellDto.ShellWeight,
                        Caliber = shellDto.Caliber
                    };

                    shells.Add(shell);
                    sb.AppendLine(string.Format(SuccessfulImportShell, shell.Caliber, shell.ShellWeight));
                }
            }
            else
            {
                sb.AppendLine(ErrorMessage);
                return sb.ToString().TrimEnd();
            }

            context.Shells.AddRange(shells);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var guns = new List<Gun>();

            var gunsDtos = JsonConvert.DeserializeObject<ImportGunDto[]>(jsonString);

            if (gunsDtos.Any())
            {
                foreach (var gunDto in gunsDtos)
                {
                    if (!IsValid(gunDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    GunType gunType;
                    bool isGunTypeValid = Enum.TryParse(gunDto.GunType, out gunType);
                    if (!isGunTypeValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool alreadyExists = guns.Any(g => g.ManufacturerId == gunDto.ManufacturerId
                                                    && g.GunWeight == gunDto.GunWeight
                                                    && g.BarrelLength == gunDto.BarrelLength
                                                    && g.NumberBuild == gunDto.NumberBuild
                                                    && g.Range == gunDto.Range
                                                    && g.GunType == gunType
                                                    && g.ShellId == gunDto.ShellId);

                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }    

                    var gun = new Gun()
                    {
                        ManufacturerId = gunDto.ManufacturerId,
                        GunWeight = gunDto.GunWeight,
                        BarrelLength = gunDto.BarrelLength,
                        NumberBuild = gunDto.NumberBuild,
                        Range = gunDto.Range,
                        GunType = gunType,
                        ShellId = gunDto.ShellId               
                    };

                    if (gunDto.Countries.Any())
                    {
                        foreach (var countryDto in gunDto.Countries)
                        {
                            var country = context.Countries.FirstOrDefault(c => c.Id == countryDto.Id);

                            gun.CountriesGuns.Add(new CountryGun
                            {
                                Country = country,
                                Gun = gun                            
                            });
                        }
                    }

                    guns.Add(gun);
                    sb.AppendLine(string.Format(SuccessfulImportGun, gun.GunType, gun.GunWeight, gun.BarrelLength));
                }
                
            }

            context.Guns.AddRange(guns);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
