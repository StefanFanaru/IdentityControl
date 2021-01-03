import { BaseIdentityModel } from '../baseIdentityModel';

export interface Secret extends BaseIdentityModel {
  value: string;
  description: string;
  type: SecretType;
  expiration: any;
  created: Date;
  clientName: string;
  apiResourceName: string;
}

export type SecretType = 'SharedSecret' | 'VisibleOneTime';
