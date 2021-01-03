export interface Segment {
  name: string;
  enabled: boolean;
  type: SegmentType;
  reference: string;
  sectorId: string;
}

export enum SegmentType {
  Endpoint,
  Method
}

export let segmentTypes: SegmentType[] = [
  SegmentType.Endpoint,
  SegmentType.Method
];
