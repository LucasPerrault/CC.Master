import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
	name: 'accountingAmount',
})
export class AccountingAmountPipe implements PipeTransform {
	public transform(value: any): string {
		const amountWithoutSign = value.replace(/[^\d,.]/g, '');
		const split = amountWithoutSign.split(',');
		const integral = split[0];
		const decimal = split[1];
		const currency = value.substring(value.length - 2);
		if (value.charAt(0) !== '-') {
			return `${integral}<span class="decimal-part">,${decimal}</span> ${currency}`;
		}

		return `(${integral}<span class="decimal-part">,${decimal}</span>)${currency}`;
	}
}
