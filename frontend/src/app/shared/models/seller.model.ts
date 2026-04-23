export interface LanguageOption {
  id: string;
  name: string;
}

export interface CreateSellerProfilePayload {
  firstName: string;
  lastName: string;
  profilePicUrl: string;
  description: string;
  languageIds: string[];
  occupation: {
    occupationName: string;
    occupationFromYear: number;
    occupationToYear: number;
  };
  skills: string[];
  educations: {
    country: string;
    institutionName: string;
    degree: string;
    major: string;
    graduationYear: number;
  }[] | null;
  certifications: {
    name: string;
    issuingOrganization: string;
    year: number;
  }[] | null;
  personalWebsite: string | null;
}

export interface UpdateSellerProfilePayload {
  firstName: string;
  lastName: string;
  profilePicUrl: string;
  description: string;
  languageIds: string[];
  occupation: {
    occupationName: string;
    occupationFromYear: number;
    occupationToYear: number;
  };
  skills: string[];
  educations: {
    country: string;
    institutionName: string;
    degree: string;
    major: string;
    graduationYear: number;
  }[] | null;
  certifications: {
    name: string;
    issuingOrganization: string;
    year: number;
  }[] | null;
  personalWebsite: string | null;
}

export interface SellerProfileFullDto {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  description: string;
  profileImageUrl: string;
  personalWebsite: string | null;
  occupation: {
    name: string;
    fromYear: number;
    toYear: number;
  };
  languages: { id: string; name: string }[];
  skills: string[];
  educations: {
    country: string;
    institutionName: string;
    degree: string;
    major: string;
    graduationYear: number;
  }[];
  certifications: {
    name: string;
    issuingOrganization: string;
    year: number;
  }[];
  createdAtUtc: string;
}
