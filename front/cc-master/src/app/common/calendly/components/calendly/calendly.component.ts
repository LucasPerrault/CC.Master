import { ChangeDetectionStrategy, Component, HostListener, Input } from '@angular/core';
import { CalendlyModalService } from '@cc/common/calendly/services/calendly-modal.service';

@Component({
  selector: 'cc-calendly',
  templateUrl: './calendly.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CalendlyComponent {
  @Input() enableOpenContactModalEvent = false;

  constructor(private modalService: CalendlyModalService) {}

  @HostListener('document:contact-us', ['$event'])
  openCalendlyModal() {
    if (this.enableOpenContactModalEvent) {
      this.modalService.openCalendlyModal();
    }
  }
}
