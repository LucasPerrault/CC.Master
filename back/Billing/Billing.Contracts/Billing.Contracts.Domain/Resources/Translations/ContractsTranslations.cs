﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by devtools.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Microsoft.Extensions.Localization;
using System;

namespace Resources.Translations
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("devtools", "1.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ContractsTranslations : IContractsTranslations
    {
        private readonly IStringLocalizer<ContractsTranslations> _stringLocalizer;

        public ContractsTranslations(IStringLocalizer<ContractsTranslations> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public string CreateOfferExceptionMessage() => _stringLocalizer["create_offer_exception_message"];
        public string CreateOfferExceptionMessage(object arg0) => _stringLocalizer["create_offer_exception_message", arg0];
        public string CreatePriceListExceptionMessage() => _stringLocalizer["create_price_list_exception_message"];
        public string CreatePriceListExceptionMessage(object arg0, object arg1) => _stringLocalizer["create_price_list_exception_message", arg0, arg1];
        public string DeleteOfferExceptionMessage() => _stringLocalizer["delete_offer_exception_message"];
        public string DeleteOfferExceptionMessage(object arg0, object arg1) => _stringLocalizer["delete_offer_exception_message", arg0, arg1];
        public string DeletePriceListExceptionMessage() => _stringLocalizer["delete_price_list_exception_message"];
        public string DeletePriceListExceptionMessage(object arg0, object arg1, object arg2) => _stringLocalizer["delete_price_list_exception_message", arg0, arg1, arg2];
        public string ImportOfferMissingHeader() => _stringLocalizer["import_offer_missing_header"];
        public string ModifyOfferExceptionMessage() => _stringLocalizer["modify_offer_exception_message"];
        public string ModifyOfferExceptionMessage(object arg0, object arg1) => _stringLocalizer["modify_offer_exception_message", arg0, arg1];
        public string ModifyPriceListExceptionMessage() => _stringLocalizer["modify_price_list_exception_message"];
        public string ModifyPriceListExceptionMessage(object arg0, object arg1, object arg2) => _stringLocalizer["modify_price_list_exception_message", arg0, arg1, arg2];
        public string OfferChangedDespiteCount() => _stringLocalizer["offer_changed_despite_count"];
        public string OldestPriceListStartDateChanged() => _stringLocalizer["oldest_price_list_start_date_changed"];
        public string PriceListChanged() => _stringLocalizer["price_list_changed"];
        public string PriceListChangedDespiteCount() => _stringLocalizer["price_list_changed_despite_count"];
        public string PriceListDetached() => _stringLocalizer["price_list_detached"];
        public string PriceListHasNegativeAmounts() => _stringLocalizer["price_list_has_negative_amounts"];
        public string PriceListShouldHaveRows() => _stringLocalizer["price_list_should_have_rows"];
        public string PriceListStartDefinedBeforeThisMonth() => _stringLocalizer["price_list_start_defined_before_this_month"];
        public string PriceListStartsOnFirstOfMonth() => _stringLocalizer["price_list_starts_on_first_of_month"];
        public string PriceListsStartsOnSameDay() => _stringLocalizer["price_lists_starts_on_same_day"];
        public string PriceRowDetached() => _stringLocalizer["price_row_detached"];
        public string PriceRowsNotOrdered() => _stringLocalizer["price_rows_not_ordered"];
        public string StartedPriceListDeleted() => _stringLocalizer["started_price_list_deleted"];

    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("devtools", "1.0.0")]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public interface IContractsTranslations
    {
        /// <summary>
        /// UNFORMATTED Similar to: Cannot create offer: {0}
        /// </summary>
        string CreateOfferExceptionMessage();
        /// <summary>
        /// Similar to: Cannot create offer: {0}
        /// </summary>
        string CreateOfferExceptionMessage(object arg0);
        /// <summary>
        /// UNFORMATTED Similar to: Cannot create price list for offer {0}: {1}
        /// </summary>
        string CreatePriceListExceptionMessage();
        /// <summary>
        /// Similar to: Cannot create price list for offer {0}: {1}
        /// </summary>
        string CreatePriceListExceptionMessage(object arg0, object arg1);
        /// <summary>
        /// UNFORMATTED Similar to: Cannot archive offer {0}: {1}
        /// </summary>
        string DeleteOfferExceptionMessage();
        /// <summary>
        /// Similar to: Cannot archive offer {0}: {1}
        /// </summary>
        string DeleteOfferExceptionMessage(object arg0, object arg1);
        /// <summary>
        /// UNFORMATTED Similar to: Cannot delete price list {0} for offer {1}: {2}
        /// </summary>
        string DeletePriceListExceptionMessage();
        /// <summary>
        /// Similar to: Cannot delete price list {0} for offer {1}: {2}
        /// </summary>
        string DeletePriceListExceptionMessage(object arg0, object arg1, object arg2);
        /// <summary>
        /// Similar to: The following headers are missing : 
        /// </summary>
        string ImportOfferMissingHeader();
        /// <summary>
        /// UNFORMATTED Similar to: Cannot modify offer {0}: {1}
        /// </summary>
        string ModifyOfferExceptionMessage();
        /// <summary>
        /// Similar to: Cannot modify offer {0}: {1}
        /// </summary>
        string ModifyOfferExceptionMessage(object arg0, object arg1);
        /// <summary>
        /// UNFORMATTED Similar to: Cannot modify price list {0} for offer {1}: {2}
        /// </summary>
        string ModifyPriceListExceptionMessage();
        /// <summary>
        /// Similar to: Cannot modify price list {0} for offer {1}: {2}
        /// </summary>
        string ModifyPriceListExceptionMessage(object arg0, object arg1, object arg2);
        /// <summary>
        /// Similar to: The properties of an offer cannot be modified if a related contract has a count
        /// </summary>
        string OfferChangedDespiteCount();
        /// <summary>
        /// Similar to: The start date of the oldest price list can never be changed
        /// </summary>
        string OldestPriceListStartDateChanged();
        /// <summary>
        /// Similar to: The properties of a price list or its rows cannot be modified via this query
        /// </summary>
        string PriceListChanged();
        /// <summary>
        /// Similar to: The properties of a price list or its rows cannot be modified if a related contract has a count (except for the addition on top of the highest bracket)
        /// </summary>
        string PriceListChangedDespiteCount();
        /// <summary>
        /// Similar to: A price list attached to an offer cannot be detached from this offer
        /// </summary>
        string PriceListDetached();
        /// <summary>
        /// Similar to: The list has at least one negative amount.
        /// </summary>
        string PriceListHasNegativeAmounts();
        /// <summary>
        /// Similar to: A price list must have rows
        /// </summary>
        string PriceListShouldHaveRows();
        /// <summary>
        /// Similar to: The start date of a price list cannot be created or updated before the current month
        /// </summary>
        string PriceListStartDefinedBeforeThisMonth();
        /// <summary>
        /// Similar to: The start date of a price list must be the first of the month
        /// </summary>
        string PriceListStartsOnFirstOfMonth();
        /// <summary>
        /// Similar to: An offer cannot have two price lists on the same start date
        /// </summary>
        string PriceListsStartsOnSameDay();
        /// <summary>
        /// Similar to: A price row attached to a price list cannot change price list
        /// </summary>
        string PriceRowDetached();
        /// <summary>
        /// Similar to: The lines of the price list are not correctly ordered.
        /// </summary>
        string PriceRowsNotOrdered();
        /// <summary>
        /// Similar to: A price list can no longer be deleted if its start date has passed
        /// </summary>
        string StartedPriceListDeleted();
    }
}