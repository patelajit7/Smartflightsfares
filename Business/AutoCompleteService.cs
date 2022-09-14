using Common;
using Infrastructure.Entities;
using Infrastructure.HelpingModel;
using Infrastructure.HelpingModel.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class AutoCompleteService
    {
        public static List<AirportAutoComplete> GetAutoSuggestions(string _searchAirport)
        {
            List<AirportAutoComplete> response = new List<AirportAutoComplete>();
            try
            {
                AirportAutoComplete airAutoSuggestion = null;
                string AUTO_TEMPLATE = "{0} - {1}, {2}, {3}";
                string code = _searchAirport.Trim().ToUpper();
                if (code.Length == 3)
                {
                    #region Code
                    if (Utility.MultiAirportCityCode.Contains(code))
                    {
                        var cityAirport = Utility.Airports.Where<Airports>(o => o.CityCode.Equals(code, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).ToList<Airports>();
                        if (cityAirport != null)
                        {
                            airAutoSuggestion = new AirportAutoComplete()
                            {
                                IsMultiAirport = true,
                                AutoSuggestion = string.Format(AUTO_TEMPLATE, code, cityAirport[0].City + " All Airports", cityAirport[0].City, cityAirport[0].CountryName),
                                TreePosition = 0,
                                Code = code,
                                Name = cityAirport[0].City,
                                Country = cityAirport[0].CountryName

                            };
                            response.Add(airAutoSuggestion);
                            foreach (var item in cityAirport)
                            {
                                airAutoSuggestion = new AirportAutoComplete()
                                {
                                    IsMultiAirport = true,
                                    AutoSuggestion = string.Format(AUTO_TEMPLATE, item.AirportCode, item.AirportName, item.City, item.CountryName),
                                    TreePosition = 1,
                                    Code = item.AirportCode,
                                    Name = item.City,
                                    Country = item.CountryName

                                };
                                response.Add(airAutoSuggestion);
                            }
                        }
                    }
                    else
                    {
                        var cityAirport = Utility.Airports.Where<Airports>(o => o.AirportCode.Equals(code, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).ToList<Airports>();
                        foreach (var item in cityAirport)
                        {
                            airAutoSuggestion = new AirportAutoComplete()
                            {
                                IsMultiAirport = false,
                                AutoSuggestion = string.Format(AUTO_TEMPLATE, item.AirportCode, item.AirportName, item.City, item.CountryName),
                                TreePosition = 0,
                                Code = item.AirportCode,
                                Name = item.City,
                                Country = item.CountryName
                            };
                            response.Add(airAutoSuggestion);
                        }
                    }

                    var cityAirPartial = Utility.Airports.Where<Airports>(o => (o.City.StartsWith(code, StringComparison.OrdinalIgnoreCase)
                        || o.AirportName.StartsWith(code, StringComparison.OrdinalIgnoreCase))
                        && !o.CityCode.Equals(code, StringComparison.OrdinalIgnoreCase)
                        && !o.AirportCode.Equals(code, StringComparison.OrdinalIgnoreCase)
                        ).OrderBy(o => o.PriorityIndex).ToList<Airports>();


                    foreach (var item in cityAirPartial)
                    {
                        airAutoSuggestion = new AirportAutoComplete()
                        {
                            IsMultiAirport = false,
                            AutoSuggestion = string.Format(AUTO_TEMPLATE, item.AirportCode, item.AirportName, item.City, item.CountryName),
                            TreePosition = 0,
                            Code = item.AirportCode,
                            Name = item.City,
                            Country = item.CountryName
                        };
                        response.Add(airAutoSuggestion);
                    }

                    #endregion

                }
                else
                {
                    if (Utility.MultiAirportCityName.Contains(code))
                    {
                        var cityAirport = Utility.Airports.Where<Airports>(o => o.City.Equals(code, StringComparison.OrdinalIgnoreCase)).OrderBy(o => o.PriorityIndex).ToList<Airports>();
                        if (cityAirport != null)
                        {
                            airAutoSuggestion = new AirportAutoComplete()
                            {
                                IsMultiAirport = true,
                                AutoSuggestion = string.Format(AUTO_TEMPLATE, cityAirport[0].CityCode, cityAirport[0].City + " All Airports", cityAirport[0].City, cityAirport[0].CountryName),
                                TreePosition = 0,
                                Code = code,
                                Name = cityAirport[0].City,
                                Country = cityAirport[0].CountryName

                            };
                            response.Add(airAutoSuggestion);
                            foreach (var item in cityAirport)
                            {
                                airAutoSuggestion = new AirportAutoComplete()
                                {
                                    IsMultiAirport = true,
                                    AutoSuggestion = string.Format(AUTO_TEMPLATE, item.AirportCode, item.AirportName, item.City, item.CountryName),
                                    TreePosition = 1,
                                    Code = item.AirportCode,
                                    Name = item.City,
                                    Country = item.CountryName

                                };
                                response.Add(airAutoSuggestion);
                            }
                        }
                    }
                    else
                    {
                        var cityAirPartial = Utility.Airports.Where<Airports>(o => (o.City.StartsWith(code, StringComparison.OrdinalIgnoreCase)
                        || o.AirportName.StartsWith(code, StringComparison.OrdinalIgnoreCase))
                        || (code.Length <= 3 && o.CityCode.StartsWith(code, StringComparison.OrdinalIgnoreCase))
                        || (code.Length <= 3 && o.AirportCode.StartsWith(code, StringComparison.OrdinalIgnoreCase))
                        || (code.Length > 3 && !string.IsNullOrEmpty(o.StateName) && o.StateName.ToUpper().StartsWith(code, StringComparison.OrdinalIgnoreCase))
                        ).OrderBy(o => o.PriorityIndex).ToList<Airports>();


                        foreach (var item in cityAirPartial)
                        {
                            airAutoSuggestion = new AirportAutoComplete()
                            {
                                IsMultiAirport = false,
                                AutoSuggestion = string.Format(AUTO_TEMPLATE, item.AirportCode, item.AirportName, item.City, item.CountryName),
                                TreePosition = 0,
                                Code = item.AirportCode,
                                Name = item.City,
                                Country = item.CountryName
                            };
                            response.Add(airAutoSuggestion);
                        }
                    }
                }
                response = response.Take(15).ToList<AirportAutoComplete>();

            }
            catch (Exception ex)
            {

                Utility.Logger.Error("AutoCompleteService.GetAirports:Exception:" + ex.ToString());
            }
            return response;
        }
        public static List<Airline> GetAirlineSuggestion(string _airline)
        {
            List<Airline> lstAirline = new List<Airline>();
            List<Airlines> tempLstAirline = null;
            Airlines tempAirline = null;
            if (!string.IsNullOrEmpty(_airline))
            {
                if (_airline.Length == 2)
                {
                    tempAirline = Utility.Airlines.Where<Airlines>(o => o.Code.Equals(_airline, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<Airlines>();
                    if (tempAirline != null)
                    {
                        lstAirline.Add(new Airline() { Code = tempAirline.Code, Name = tempAirline.Name });
                    }
                    tempLstAirline = Utility.Airlines.Where<Airlines>(o => o.Name.StartsWith(_airline, StringComparison.OrdinalIgnoreCase)).ToList<Airlines>();
                    if (tempLstAirline != null && tempLstAirline.Count > 0)
                    {
                        foreach (Airlines item in tempLstAirline)
                        {
                            lstAirline.Add(new Airline() { Code = item.Code, Name = item.Name });
                        }

                    }
                }
                else
                {
                    tempLstAirline = Utility.Airlines.Where<Airlines>(o => o.Name.StartsWith(_airline, StringComparison.OrdinalIgnoreCase)).ToList<Airlines>();
                    if (tempLstAirline != null && tempLstAirline.Count > 0)
                    {
                        foreach (Airlines item in tempLstAirline)
                        {
                            lstAirline.Add(new Airline() { Code = item.Code, Name = item.Name });
                        }

                    }
                }
            }

            return lstAirline;
        }
    }
}
