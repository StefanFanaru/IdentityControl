import { BaseIdentityModel } from '../baseIdentityModel';
import { BaseOption } from '../option';

export interface ApiScope extends BaseIdentityModel {
  name: string;
  displayName: string;
  description: string;
}

export interface ApiScopeOption extends BaseOption<number> {
  selected: boolean;
}
