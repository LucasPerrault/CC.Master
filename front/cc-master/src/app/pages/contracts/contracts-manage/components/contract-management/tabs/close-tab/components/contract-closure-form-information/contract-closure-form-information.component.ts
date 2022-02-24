import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { isAfter } from 'date-fns';

import { IClosureFormValidationContext } from '../../models/closure-form-validation-context.interface';

@Component({
  selector: 'cc-contract-closure-form-information',
  templateUrl: './contract-closure-form-information.component.html',
})
export class ContractClosureFormInformationComponent {
  @Input() context: IClosureFormValidationContext;

  constructor(private translatePipe: TranslatePipe, private datePipe: DatePipe) { }

  public getMinDateReason(): string {
    const attachmentDate = this.getAttachmentDate();

    if (!!this.context?.mostRecentAttachment && !!this.context?.lastCountPeriod) {
      return isAfter(attachmentDate, this.context.lastCountPeriod)
        ? this.getAttachmentDateInformation(attachmentDate)
        : this.getLastCountDateInformation();
    }

    if (!!this.context?.mostRecentAttachment) {
      return this.getAttachmentDateInformation(attachmentDate);
    }

    if (!!this.context.lastCountPeriod) {
      return this.getLastCountDateInformation();
    }

    return this.getTheoreticalStartOnInformation();
  }

  private getAttachmentDateInformation(attachmentDate: Date): string {
    const translationKey = !!this.context.mostRecentAttachment?.end
      ? 'contracts_closeDate_blockingAttachment_until'
      : 'contracts_closeDate_blockingAttachment_since';

    return this.translatePipe.transform(translationKey, {
      name: this.context.mostRecentAttachment.legalEntity.name,
      id: this.context.mostRecentAttachment.legalEntity.id,
      date: this.datePipe.transform(attachmentDate, 'MMMM yyyy'),
    });
  }

  private getLastCountDateInformation(): string {
    const lastCountDate = this.datePipe.transform(this.context.lastCountPeriod, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_lastCountDate_callout';
    return this.translatePipe.transform(translationKey, { date: lastCountDate });
  }

  private getTheoreticalStartOnInformation(): string {
    const theoreticalStartDate = this.datePipe.transform(this.context.theoreticalStartOn, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_theoreticalStartOn_callout';
    return this.translatePipe.transform(translationKey, { date: theoreticalStartDate });
  }

  private getAttachmentDate(): Date {
    if (!this.context.mostRecentAttachment) {
      return null;
    }

    const hasEndDate = !!this.context.mostRecentAttachment?.end;
    return hasEndDate? new Date(this.context.mostRecentAttachment.end) : new Date(this.context.mostRecentAttachment.start);
  }

}
