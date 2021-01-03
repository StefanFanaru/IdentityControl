export interface ApplicationUser {
  id: string;
  lastName: string;
  firstName: string;
  email: string;
  insertDate: string;
  disableDate: string | null;
  county: string;
  city: string;
  acceptsInformativeEmails: boolean;
  hasPicture: boolean | null;
  pictureUrl: string;
  emailConfirmed: boolean;
  phoneNumber: string;
  phoneNumberConfirmed: boolean;
  twoFactorEnabled: boolean;
}

export interface ApplicationUserList {
  id: string;
  lastName: string;
  firstName: string;
  insertDate: string;
  disableDate: string | null;
  deleteDate: string | null;
  county: string;
  city: string;
  emailConfirmed: boolean;
}
