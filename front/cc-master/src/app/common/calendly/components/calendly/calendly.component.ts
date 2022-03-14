import { ChangeDetectionStrategy, Component, HostListener, Input } from '@angular/core';

@Component({
  selector: 'cc-calendly',
  templateUrl: './calendly.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CalendlyComponent {
  @Input() enableOpenContactModalEvent = false;

  @HostListener('document:contact-us', ['$event'])
  openCalendlyModal() {
    if (this.enableOpenContactModalEvent) {
      console.log('open calendly modal');
    }
  }
}
