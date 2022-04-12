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
    public class CmrrTranslations : ICmrrTranslations
    {
        private readonly IStringLocalizer<CmrrTranslations> _stringLocalizer;

        public CmrrTranslations(IStringLocalizer<CmrrTranslations> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer ?? throw new ArgumentNullException(nameof(stringLocalizer));
        }

        public string CmrrExportContraction() => _stringLocalizer["cmrr_export_contraction"];
        public string CmrrExportCreation() => _stringLocalizer["cmrr_export_creation"];
        public string CmrrExportEvolutionAmount() => _stringLocalizer["cmrr_export_evolution_amount"];
        public string CmrrExportEvolutionAmountType() => _stringLocalizer["cmrr_export_evolution_amount_type"];
        public string CmrrExportEvolutionBu() => _stringLocalizer["cmrr_export_evolution_bu"];
        public string CmrrExportEvolutionProduct() => _stringLocalizer["cmrr_export_evolution_product"];
        public string CmrrExportEvolutionSolution() => _stringLocalizer["cmrr_export_evolution_solution"];
        public string CmrrExportExpansion() => _stringLocalizer["cmrr_export_expansion"];
        public string CmrrExportSituationAxis() => _stringLocalizer["cmrr_export_situation_axis"];
        public string CmrrExportSituationChurn() => _stringLocalizer["cmrr_export_situation_churn"];
        public string CmrrExportSituationNrr() => _stringLocalizer["cmrr_export_situation_nrr"];
        public string CmrrExportSituationVariation() => _stringLocalizer["cmrr_export_situation_variation"];
        public string CmrrExportTermination() => _stringLocalizer["cmrr_export_termination"];
        public string CmrrExportTotalFormat() => _stringLocalizer["cmrr_export_total_format"];
        public string CmrrExportTotalFormat(object arg0) => _stringLocalizer["cmrr_export_total_format", arg0];
        public string CmrrExportUpsell() => _stringLocalizer["cmrr_export_upsell"];

    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("devtools", "1.0.0")]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public interface ICmrrTranslations
    {
        /// <summary>
        /// Similar to: Contraction
        /// </summary>
        string CmrrExportContraction();
        /// <summary>
        /// Similar to: Acquisition
        /// </summary>
        string CmrrExportCreation();
        /// <summary>
        /// Similar to: Starting MRR
        /// </summary>
        string CmrrExportEvolutionAmount();
        /// <summary>
        /// Similar to: Amount type
        /// </summary>
        string CmrrExportEvolutionAmountType();
        /// <summary>
        /// Similar to: BU
        /// </summary>
        string CmrrExportEvolutionBu();
        /// <summary>
        /// Similar to: Product
        /// </summary>
        string CmrrExportEvolutionProduct();
        /// <summary>
        /// Similar to: Solution
        /// </summary>
        string CmrrExportEvolutionSolution();
        /// <summary>
        /// Similar to: Expansion
        /// </summary>
        string CmrrExportExpansion();
        /// <summary>
        /// Similar to: Axis
        /// </summary>
        string CmrrExportSituationAxis();
        /// <summary>
        /// Similar to: Churn Rate
        /// </summary>
        string CmrrExportSituationChurn();
        /// <summary>
        /// Similar to: NRR
        /// </summary>
        string CmrrExportSituationNrr();
        /// <summary>
        /// Similar to: Variation
        /// </summary>
        string CmrrExportSituationVariation();
        /// <summary>
        /// Similar to: Termination
        /// </summary>
        string CmrrExportTermination();
        /// <summary>
        /// UNFORMATTED Similar to: Total {0}
        /// </summary>
        string CmrrExportTotalFormat();
        /// <summary>
        /// Similar to: Total {0}
        /// </summary>
        string CmrrExportTotalFormat(object arg0);
        /// <summary>
        /// Similar to: Upsell
        /// </summary>
        string CmrrExportUpsell();
    }
}