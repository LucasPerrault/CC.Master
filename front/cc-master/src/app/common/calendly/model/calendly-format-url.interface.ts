import { HttpParams } from '@angular/common/http';

export enum CalendlyEmbedType {
  Inline = 'Inline',
  PopupWidget = 'PopupWidget',
  PopupButton = 'PopupButton',
}

export interface ICalendlyPrefill {
  name: string;
  email: string;
  firstName: string;
  lastName: string;
  location: string;
  guests: string[];
}

enum PrefillQueryKey {
  Name = 'name',
  Location = 'location',
  FirstName = 'first_name',
  LastName = 'last_name',
  Guests = 'guests',
  Email = 'email'
}

enum EmbedTypeQueryKey {
  EmbedType = 'embed_type',
}

export class CalendlyQueryMapping {
  public static toHttpParams(embedType?: CalendlyEmbedType, prefill?: ICalendlyPrefill): HttpParams {
    let httpParams = new HttpParams();
    httpParams = httpParams.set('embed_domain', 1); // embed_domain must be defined to receive messages from the Calendly iframe

    httpParams = !!embedType ? httpParams.set(EmbedTypeQueryKey.EmbedType, embedType) : httpParams;
    return !!prefill ? this.setPrefillHttpParams(prefill, httpParams) : httpParams;
  }

  private static setPrefillHttpParams(prefill: ICalendlyPrefill, params: HttpParams): HttpParams {
    params = prefill.name ? params.set(PrefillQueryKey.Name, encodeURIComponent(prefill.name)) : params;
    params = prefill.firstName ? params.set(PrefillQueryKey.FirstName, encodeURIComponent(prefill.firstName)) : params;
    params = prefill.lastName ? params.set(PrefillQueryKey.LastName, encodeURIComponent(prefill.lastName)) : params;
    params = prefill.email ? params.set(PrefillQueryKey.Email, prefill.email) : params;
    params = prefill.location ? params.set(PrefillQueryKey.Location, encodeURIComponent(prefill.location)) : params;
    params = prefill.guests ? params.set(PrefillQueryKey.Guests, prefill.guests.join(',')) : params;
    return params;
  }
}
