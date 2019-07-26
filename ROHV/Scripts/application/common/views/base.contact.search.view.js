define(function (require) {

    require('marionette');
    require('underscore');
    var BaseView = require('views/base.view');
    return BaseView.extend({

        setMedicalService: function (id, agency) {
            var _this = this;
            $(id).selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/servicecoordinatorslist/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        var agencyName = agency || 'AgencyName';
                        var agencyId = $('#' + agencyName).val();
                        if (agencyId != null && agencyId.length > 0) {
                            params.AgencyId = agencyId;
                        }
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search a Service Coordinator...'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ContactId,
                                    'text': curr.LastName + ", " + curr.FirstName,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.CompanyName
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: false,
                requestDelay: 100,
                emptyRequest: true
            });
        },
    });/*.end module*/

});/*.end defined*/