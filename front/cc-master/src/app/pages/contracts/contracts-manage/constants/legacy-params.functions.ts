import { Params } from '@angular/router';

const standardParamIndicator = '?';
const legacyParamsSeparator = ';';
const legacyParamKeyValueSeparator = '=';

export function getUrlWithoutParams(url: string): string {
    return url.split(legacyParamsSeparator)[0];
}

export function getLegacyParams(url: string): Params {
    const params: Params = { };
    const legacyUrl = removeNonLegacyParams(url);
    const keyValuePairs = getKeyValuePairs(legacyUrl);
    for (const kvpAsAsString of keyValuePairs) {
        const kvp = kvpAsAsString.split(legacyParamKeyValueSeparator);
        params[cleanupLegacyKey(kvp[0])] = kvp[1];
    }
    return params;
}

export function hasLegacyParams(url: string): boolean {
    return url.includes(legacyParamsSeparator);
}

function cleanupLegacyKey(key: string): string {
    // standard keys are all lower, legacy keys are pascalCase
    return key.toLowerCase();
}

function removeNonLegacyParams(url: string): string {
    return  url.split(standardParamIndicator)[0];
}

function getKeyValuePairs(url: string): string[] {
    const keyValuePairs = url.split(legacyParamsSeparator);
    keyValuePairs.shift(); // first entry is url, remove it
    return keyValuePairs;
}
