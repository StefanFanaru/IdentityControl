import { BaseIdentityModel } from '../baseIdentityModel';

export interface ApiResource extends BaseIdentityModel {
  name: string;
  displayName: string;
  description: string;
  created: string;
  updated: Date;
}
