import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import { IClosureFormValidationContext, IContextAttachment } from '../../models/closure-form-validation-context.interface';

@Component({
  selector: 'cc-contract-closure-form-information',
  templateUrl: './contract-closure-form-information.component.html',
})
export class ContractClosureFormInformationComponent {
  @Input() context: IClosureFormValidationContext;

  constructor(private translatePipe: TranslatePipe, private datePipe: DatePipe) { }

  public getAttachmentDateInformation(attachment: IContextAttachment): string {
    const hasEndDate = !!attachment?.end;
    const mostRecentDate = hasEndDate ? attachment.end : attachment.start;
    const translationKey = hasEndDate ? 'contracts_closeDate_blockingAttachment_until' : 'contracts_closeDate_blockingAttachment_since';

    return this.translatePipe.transform(translationKey, {
      name: attachment.legalEntity.name,
      id: attachment.legalEntity.id,
      date: this.datePipe.transform(mostRecentDate, 'MMMM yyyy'),
    });
  }

  public getLastCountDateInformation(): string {
    const lastCountDate = this.datePipe.transform(this.context.lastCountPeriod, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_lastCountDate_callout';
    return this.translatePipe.transform(translationKey, { date: lastCountDate });
  }

  public getTheoreticalStartOnInformation(): string {
    const theoreticalStartDate = this.datePipe.transform(this.context.theoreticalStartOn, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_theoreticalStartOn_callout';
    return this.translatePipe.transform(translationKey, { date: theoreticalStartDate });
  }

}
