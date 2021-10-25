import { ELuDateGranularity } from '@lucca-front/ng/core';

interface IDateConfiguration {
  min?: Date;
  max?: Date;
  class?: string;
}

export enum EndDateGranularityPolicy {
  Beginning,
  End,
}

export interface IDateRangeConfiguration {
  startDateConfiguration: IDateConfiguration;
  endDateConfiguration: IDateConfiguration;
  granularity: ELuDateGranularity;
  periodCoverStrategy: EndDateGranularityPolicy;
}

export const defaultDateRangeConfiguration: IDateRangeConfiguration = {
  startDateConfiguration:  { },
  endDateConfiguration:  { },
  periodCoverStrategy: EndDateGranularityPolicy.Beginning,
  granularity: ELuDateGranularity.day,
};
