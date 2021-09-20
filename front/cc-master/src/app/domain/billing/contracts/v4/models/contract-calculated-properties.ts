import { IContract, IContractAttachment } from '@cc/domain/billing/contracts/v4';
import { isBefore } from 'date-fns';

export class ContractCalculatedProperties {
  // @ts-ignore
  public static name(contract: IContract): string {
    const clientName = contract.client?.name ?? '?';
    const distributorName = contract.distributor?.name.split(' - ')[1] ?? '?';

    const productCode = contract.commercialOffer?.product?.code ?? '?';
    const startedYear = ContractCalculatedProperties.startOn(contract.attachments, contract.theoreticalStartOn)?.getFullYear();
    const startOnCode = startedYear?.toString()?.substr(2, 2) ?? '?';
    const productAndStartOnCode = `${ productCode }${ startOnCode }`;

    return [clientName, distributorName, productAndStartOnCode].join(' - ');
  }

  public static startOn(attachments: IContractAttachment[], theoreticalStartOn: string): Date {
    if (!attachments.length) {
      return !!theoreticalStartOn ? new Date(theoreticalStartOn) : null;
    }

    const oldestAttachment = attachments.reduce((previous, current) =>
      isBefore(new Date(previous?.startsOn), new Date(current?.startsOn)) ? previous : current,
    );

    return new Date(oldestAttachment?.startsOn);
  }
}
