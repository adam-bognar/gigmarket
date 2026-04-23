import {AbstractControl, ValidationErrors} from '@angular/forms';

export function yearRangeValidator(control: AbstractControl): ValidationErrors | null {
  const from = Number(control.get('ofrom')?.value);
  const to = Number(control.get('oto')?.value);
  if (from && to && to < from) return { yearRange: true };
  return null;
}

export const CURRENT_YEAR = 2026;

export function buildYearList(from = 1970, to = CURRENT_YEAR): number[] {
  const result: number[] = [];
  for (let y = to; y >= from; y--) result.push(y);
  return result;
}

export const DEGREE_OPTIONS = [
  'High School Diploma', 'Associate Degree', 'BSc', 'BA', 'BEng',
  'MSc', 'MA', 'MEng', 'MBA', 'PhD', 'MD', 'JD', 'Other',
];

export const COUNTRY_OPTIONS = [
  'Afghanistan','Albania','Algeria','Andorra','Angola','Argentina','Armenia','Australia','Austria','Azerbaijan',
  'Bahamas','Bahrain','Bangladesh','Belarus','Belgium','Belize','Benin','Bhutan','Bolivia','Bosnia and Herzegovina',
  'Botswana','Brazil','Brunei','Bulgaria','Burkina Faso','Burundi','Cambodia','Cameroon','Canada','Chad','Chile',
  'China','Colombia','Congo','Costa Rica','Croatia','Cuba','Cyprus','Czech Republic','Denmark','Ecuador','Egypt',
  'El Salvador','Estonia','Ethiopia','Finland','France','Georgia','Germany','Ghana','Greece','Guatemala','Honduras',
  'Hungary','India','Indonesia','Iran','Iraq','Ireland','Israel','Italy','Jamaica','Japan','Jordan','Kazakhstan',
  'Kenya','Kuwait','Kyrgyzstan','Latvia','Lebanon','Libya','Lithuania','Luxembourg','Malaysia','Malta','Mexico',
  'Moldova','Mongolia','Montenegro','Morocco','Mozambique','Myanmar','Nepal','Netherlands','New Zealand','Nicaragua',
  'Nigeria','North Korea','Norway','Oman','Pakistan','Palestine','Panama','Paraguay','Peru','Philippines','Poland',
  'Portugal','Qatar','Romania','Russia','Rwanda','Saudi Arabia','Senegal','Serbia','Singapore','Slovakia','Slovenia',
  'Somalia','South Africa','South Korea','Spain','Sri Lanka','Sudan','Sweden','Switzerland','Syria','Taiwan',
  'Tajikistan','Tanzania','Thailand','Tunisia','Turkey','Uganda','Ukraine','United Arab Emirates','United Kingdom',
  'United States','Uruguay','Uzbekistan','Venezuela','Vietnam','Yemen','Zambia','Zimbabwe',
];
