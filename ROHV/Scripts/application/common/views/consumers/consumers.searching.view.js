define(function (require) {

    require('marionette');
    require('underscore');
    var BaseView = require('views/base.view');

    return BaseView.extend({

        container: '<div id="search-view"></div>',
        el: "#search-view",
        events: function () {
            return {
                "click #reset-search": "onReset",
                "click #reset-search": "onReset",
                "change #search-type": "onChangeSearchTypeSelector"
            }
        },
        initialize: function () { },
        appendContainer: function () {
            $("#searchingContainer").append(this.container);
        },
        template: function (serialized_model) {
            var templateHtml = $("#search-template").html();
            return templateHtml;
        },       
        onDomRefresh: function () {
            this.setConsumerField();
            this.setEmployeeField();
            this.setMedicaidField();
            this.setMscField();
            this.setTabsField();
            this.onChangeSearchTypeSelector();
            this.setCommonSearchField();
            $('#search-type').selectpicker();
        },
        onChangeSearchTypeSelector: function () {
            $(".search-container").hide();
            var selectedType = $("#search-type").val();
            $("." + selectedType).show();
        },
        setTabsField: function () {
            
            var _this = this;
            $('#tabsId').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingtabsid/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search a consumer by Tabs No'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ConsumerId,
                                    'text': curr.TABSNo,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.LastName + ", " + curr.FirstName
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: false,
                requestDelay: 100
            });          
        },
        setMscField: function () {
            var _this = this;
            $('#msc').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchservicecoordinators/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search a consumer by MSC'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ConsumerId,
                                    'text':  curr.ServiceCoordinatorLastName + ", " + curr.ServiceCoordinatorFirstName,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.ConsumerLastName + ", " + curr.ConsumerFirstName,
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: false,
                requestDelay: 100
            });           
        },
        setMedicaidField: function () {
            var _this = this;
            $('#medicaid').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingmedicaid/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search a consumer by Medicaid No'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ConsumerId,
                                    'text': curr.MedicaidNo,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.LastName + ", " + curr.FirstName
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: false,
                requestDelay: 100
            });
        },
        setConsumerField: function () {
            var _this = this;
            $('#consumer').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingconsumers/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}'
                        };
                        if (_this.employeeId != null) {
                            params.EmployeeId = _this.employeeId;
                        }
                        if (_this.consumerId != null) {
                            params.ConsumerId = _this.consumerId;
                        }
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search by consumer name'
                },
                preprocessData: function (data) {
                    var result = [];
                    if (data.hasOwnProperty('data')) {
                        var len = data.data.length;
                        for (var i = 0; i < len; i++) {
                            var curr = data.data[i];
                            result.push(
                                {
                                    'value': curr.ConsumerId,
                                    'text': curr.LastName + ', ' + curr.FirstName,
                                    'data': {
                                        'icon': 'glyphicon-user',
                                        'subtext': curr.City + ", " + curr.State
                                    },
                                    'disabled': false
                                }
                            );
                        }
                    }
                    return result;
                },
                preserveSelected: false,
                emptyRequest: true,
                requestDelay: 100
                });   
            $('#consumer').on('changed.bs.select', function (e) {
                _this.trigerUpdateData();
            });
        },
        setEmployeeField: function () {
            var _this = this;
            $('#employee').selectpicker({
                dropupAuto: false,
            }).ajaxSelectPicker({
                ajax: {
                    url: '/api/consumerapi/searchingemployee/',
                    data: function () {
                        var params = {
                            q: '{{{q}}}',
                            skipNotAssigned: true
                        };
                        return params;
                    }
                },
                locale: {
                    emptyTitle: 'Search a consumer by employee'
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
                                    'text': curr.LastName + ', ' + curr.FirstName,
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
                processData: function () {

                },
                preserveSelected: false,
                clearOnEmpty: true,
                log: false,
                requestDelay: 100
                });
            $('#employee').on('change', function (e) {
                if (_this.employeeId != null && $(e.currentTarget).val().length == 0) {
                    _this.employeeId = null;
                    $('#consumer').data()['AjaxBootstrapSelect'].list.destroy();
                }
            });
            $('#employee').on('changed.bs.select', function (e) {
                _this.employeeId = $(e.currentTarget).val();
                _this.resetAllExcept("employee");
                _this.triggerConsumerUpdate();
            });
        },
        setCommonSearchField: function () {

            var _this = this;
            $('select.regular-search', $(".search-main-block")).on('change', function (e) {
                if (_this.consumerId != null && $(e.currentTarget).val().length == 0) {
                    _this.consumerId = null;
                    $('#consumer').data()['AjaxBootstrapSelect'].list.destroy();
                }
            });
            $('select.regular-search').on('changed.bs.select', function (e) {
                _this.resetAllExcept($(this).attr('id'));
                _this.consumerId = $(e.currentTarget).val();
                _this.triggerConsumerUpdate();

            });

            $('#consumer').on('complete.request', function () {
                if (_this.consumerId != null) {
                    $('#consumer').selectpicker('val', _this.consumerId);                   
                    _this.trigerUpdateData();
                }
            });

        },
        triggerConsumerUpdate: function () {
            var list = $('#consumer').data()['AjaxBootstrapSelect'].list;
            list.destroy();
            list.cache = [];
            var event = $('#consumer').data()['AjaxBootstrapSelect'].options.bindEvent;
            $('#consumer').data()['selectpicker'].$searchbox.trigger(event);
        },
        getSelectedConsumerId: function () {
            return $("#consumer").val();
        },
        trigerUpdateData: function () {
            this.triggerMethod('update:full-refresh-data');
        },
        resetAllExcept: function (exceptId) {
            var _this = this;
            $('select.search,select.employee-search,select.main-search').each(function (i, el) {
                var id = $(el).attr('id');
                if (id !== exceptId) {
                    _this.resetField(id);
                }
            });
            
        },
        resetField: function (id) {
            var list = $('#' + id).data()['AjaxBootstrapSelect'].list;
            list.destroy();
            list.cache = [];
            $('#' + id).selectpicker('val', '');
            
            if (id == "employee") {
                this.employeeId = null;
            }
            else {
                this.consumerId = null
            };
        },
        onReset: function () {
            var _this = this;
            $('select.search,select.employee-search,select.main-search').each(function (i, el) {
                var id = $(el).attr('id');               
                _this.resetField(id);                
            });
            this.trigerUpdateData();
        }

    });

});