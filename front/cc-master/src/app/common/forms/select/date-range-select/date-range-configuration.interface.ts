import { ELuDateGranularity } from '@lucca-front/ng/core';

interface IDateConfiguration {
  min?: Date;
  max?: Date;
  class?: string;
  granularity: ELuDateGranularity;
}

export interface IDateRangeConfiguration {
  startDateConfiguration: IDateConfiguration;
  endDateConfiguration: IDateConfiguration;
}

export const defaultDateConfiguration: IDateConfiguration = {
  granularity: ELuDateGranularity.day,
};

export const defaultDateRangeConfiguration: IDateRangeConfiguration = {
  startDateConfiguration: defaultDateConfiguration,
  endDateConfiguration: defaultDateConfiguration,
};
