import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { TranslatePipe } from '@cc/aspects/translate';
import { INavigationTab, navigationTabs } from '@cc/common/navigation';
import { noNavTabs } from '@cc/common/routing';

@Injectable()
export class TitleService {

  private readonly cloudControlTitle = 'Cloud Control';
  private readonly cloudControlPageTitlePrefix = 'CC';

  constructor(
    private title: Title,
    private translatePipe: TranslatePipe,
  ) {
  }

  public updatePageTitle(activatedUrl: string): void {
    const activatedTab = this.getActivatedTab(activatedUrl);
    if (!activatedTab) {
      this.title.setTitle(this.cloudControlTitle);
      return;
    }

    const translatedName = this.translatePipe.transform(activatedTab.name);
    this.title.setTitle(`${ this.cloudControlPageTitlePrefix } - ${ translatedName }`);
  }

  private getActivatedTab(activatedUrl: string): INavigationTab | undefined {
    const firstSegment = this.getFirstSegment(activatedUrl);
    const allTabs = [...navigationTabs, ...noNavTabs];

    return allTabs.find(tab => tab.url.toLowerCase() === firstSegment.toLowerCase());
  }

  private getFirstSegment(activatedUrl: string): string {
    const urlWithNoPrependingSlash = activatedUrl.startsWith('/') ? activatedUrl.slice(1) : activatedUrl;
    return urlWithNoPrependingSlash.split('/')[0];
  }
}
