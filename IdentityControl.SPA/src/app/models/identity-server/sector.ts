import { Segment } from './segment';

export interface Sector {
  name: string;
  enabled: boolean;
  reference: string;
  type: SectorType;
  resourceId: string;
  segments: Segment[];
}

export enum SectorType {
  Controller,
  Service
}

export let sectorTypes: SectorType[] = [
  SectorType.Controller,
  SectorType.Service
];
