import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LuSidepanel } from '@lucca-front/ng/sidepanel';

@Component({
  selector: 'cc-code-sources-creation-entry-modal',
  template: '',
})
export class CodeSourcesCreationEntryModalComponent {

  constructor(private luSidepanel: LuSidepanel, private router: Router, private activatedRoute: ActivatedRoute) {
    const dialog = this.luSidepanel.open(CodeSourceCreationModalComponent);

    dialog.onClose.subscribe(async () => await this.redirectToParentAsync());
    dialog.onDismiss.subscribe(async () => await this.redirectToParentAsync());
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
    });
  }
}

@Component({
  selector: 'cc-code-source-creation-modal',
  templateUrl: './code-source-creation-modal.component.html',
})
export class CodeSourceCreationModalComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
