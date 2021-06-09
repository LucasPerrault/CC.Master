import { Component, Inject } from '@angular/core';
import { CookiesService } from '@cc/aspects/cookies';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';

@Component({
  selector: 'cc-banner',
  templateUrl: './banner.component.html',
  styleUrls: ['./banner.component.scss'],
})
export class BannerComponent {

  private readonly logoutHref = '/logout';

  constructor(
    @Inject(PRINCIPAL) public principal: IPrincipal,
    private cookiesService: CookiesService) {}

  public async logoutAsync(): Promise<void> {
    this.cookiesService.deleteAuthToken();
    window.location.href = this.logoutHref;
  }

}