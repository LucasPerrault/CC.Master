import { INavigationTab } from '@cc/common/navigation';

export enum CountsNavigationPath {
  Table = 'table',
  Launcher = 'launcher',
}

export const navigationTabs: INavigationTab[] = [
  {
    name: 'counts_table_page_title',
    url: CountsNavigationPath.Table,
  },
  {
    name: 'counts_launcher_page_title',
    url: CountsNavigationPath.Launcher,
  },
];

