import { Sector } from './sector';

export interface AccessControlResource {
  id: string;
  name: string;
  type: ResourceType;
  enabled: boolean;
  urlOrigin: string;
  sectors: Sector[];
}

export interface AccessControlResourceList {
  id: string;
  name: string;
  type: ResourceType;
  enabled: boolean;
  urlOrigin: string;
}

export enum ResourceType {
  Api,
  CommunicationService,
  InternalTool
}

export const resourceTypes: ResourceType[] = [
  ResourceType.Api,
  ResourceType.CommunicationService,
  ResourceType.InternalTool
];
