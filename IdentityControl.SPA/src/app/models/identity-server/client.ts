import { BaseIdentityModel } from '../baseIdentityModel';
import { Secret } from './secret';

export interface Client extends BaseIdentityModel {
  name: string;
  displayName: string;
  requireClientSecret: boolean;
  clientUri: string;
  requirePkce: boolean;
  allowAccessTokensViaBrowser: boolean;
  description: string;
  frontChannelLogoutUri: string;
  backChannelLogoutUri: string;
  allowOfflineAccess: boolean;
  identityTokenLifetime: number;
  allowedIdentityTokenSigningAlgorithms: string;
  accessTokenLifetime: number;
  authorizationCodeLifetime: number;
  includeJwtId: boolean;
  created: string;
  updated: string | null;
  lastAccessed: string | null;
  clientSecrets: Secret[];
}

export interface ClientChild {
  id: number;
  value: string;
}

export enum ClientChildType {
  GrantType,
  CorsOrigin,
  RedirectUri,
  LogoutRedirectUri
}
