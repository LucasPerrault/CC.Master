import { INavigationTab } from '@cc/common/navigation';

export enum OfferEditionNavigationPath {
  Edit = 'edit',
  PriceLists = 'priceLists',
}

export const navigationTabs: INavigationTab[] = [
  {
    name: 'offers_edition_navigation_information',
    url: OfferEditionNavigationPath.Edit,
  },
  {
    name: 'offers_edition_navigation_priceLists',
    url: OfferEditionNavigationPath.PriceLists,
  },
];
