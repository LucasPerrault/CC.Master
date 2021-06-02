import { Injectable } from '@angular/core';

/* Zendesk global variable
 * It initialized by the script (src/assets/zendesk.js) and configured in angular.json file
 */
declare const zE: any;

@Injectable()
export class ZendeskHelpService {

  public setupWebWidget(): void {
    zE('webWidget', 'hide');
  }

  public toggleWidgetDisplay(): void {
    zE(() => {
      zE.activate({ hideOnClose: true,  webWidget: { zIndex: 9999 } });
    });
  }
}
