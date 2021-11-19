import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { Subject } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { OffersDataService } from '../../services/offers-data.service';
import { IUploadedOffer } from './uploaded-offer-dto.interface';

@Component({
  selector: 'cc-offer-import',
  templateUrl: './offer-import.component.html',
  styleUrls: ['./offer-import.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferImportComponent implements OnInit {

  public isReadonly: FormControl = new FormControl(true);
  public formControl: FormControl = new FormControl([]);

  public buttonClass$: Subject<string> = new Subject<string>();

  public get totalCount(): number {
    return this.formControl.value?.length;
  }

  constructor(private router: Router, private dataService: OffersDataService) { }

  ngOnInit(): void {
  }

  public upload(offers: IUploadedOffer[]): void {
    this.formControl.setValue(offers);
  }

  public create(): void {
    this.dataService.createRange$(this.formControl.value)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(this.buttonClass$);
  }

  public cancel(): void {
    this.router.navigate([NavigationPath.Offers])
      .then(() => null);
  }

}
