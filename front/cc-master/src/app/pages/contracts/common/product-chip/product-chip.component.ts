import { Component, Input } from '@angular/core';
import { IProduct, ProductId } from '@cc/domain/billing/offers';

@Component({
  selector: 'cc-product-chip',
  templateUrl: './product-chip.component.html',
  styleUrls: ['./product-chip.component.scss'],
})
export class ProductChipComponent {
  @Input() public product: IProduct;

  public getProductCss(productId: number): string {
    return `mod-${ this.getProductCssKey(productId) }`;
  }

  private getProductCssKey(productId: number): string {
    switch (productId) {
      case ProductId.Figgo:
      case ProductId.FiggoGXP:
        return 'figgo';
      case ProductId.Cleemy:
      case ProductId.CleemyBanking:
      case ProductId.CleemyWithCAndC:
      case ProductId.CaptureCollect:
      case ProductId.BudgetInsight:
        return 'cleemy';
      case ProductId.Pagga:
        return 'pagga';
      case ProductId.TimmiTS:
      case ProductId.TimesheetOld:
        return 'timmi-timesheet';
      case ProductId.TimmiProj:
        return 'timmi-project';
      case ProductId.PopleeCoreHR:
      case ProductId.PopleCoreRhWithSignature:
        return 'poplee-core-rh';
      case ProductId.PopleeComp:
        return 'poplee-rem';
      case ProductId.PopleeGoals:
        return 'poplee-entretiens';
      case ProductId.SuiteSIRHPaie:
        return 'sirh-paie';
      case ProductId.SuiteEssentielSIRH:
      case ProductId.SuiteEssentielRRHH:
        return 'essentiel';
      case ProductId.SuiteStartup:
        return 'startup';
      case ProductId.NikoNiko:
        return 'nikoniko';
      default:
        return 'others';
    }
  }

}
