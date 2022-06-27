namespace TALXIS.Sales.Apps.Default.talxis_salesquoteheader {
    export class Dialogs {
        public static async DialogAcceptQuote(executionContext: Xrm.Events.EventContext | Xrm.FormContext) {
            var formContext: Xrm.FormContext = TALXIS.Utility.Apps.Start.UCIClientExtensions.TryGetFormContext(executionContext);
            formContext.ui.close();
            Xrm.Utility.showProgressIndicator("Accepting Quote");
            var talxis_acceptquote = {
                entity: {
                    id: formContext.getAttribute("talxis_salesquoteheaderid").getValue(),
                    entityType: "talxis_salesquoteheader"
                },
                getMetadata: function () {
                    return {
                        boundParameter: "entity",
                        parameterTypes: {
                            "entity": {
                                "typeName": "mscrm.talxis_salesquoteheader",
                                "structuralProperty": 5
                            }
                        },
                        operationType: 0,
                        operationName: "talxis_closeasaccepted"
                    };
                }
            };
            var response: Xrm.ExecuteResponse;
            try {
                response = await Xrm.WebApi.online.execute(talxis_acceptquote);
            } catch (e) {
                Xrm.Utility.closeProgressIndicator();
                Xrm.Navigation.openAlertDialog({ text: e.message, title: "Error" })
                return;
            }
            Xrm.Utility.closeProgressIndicator();
            parent.window.location.reload();
        }
    }
}