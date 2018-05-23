
var myTreeData;
$.ajax({
    type: 'POST',
    url: "/Area/GetAllItems",
    data: {},
    async: false,
    dataType: 'json',
    success: function (data) {
        myTreeData = data;
        (function () {
            var app;
            var tree;

            app = angular.module('treeGridTest', ['treeGrid']);
            //console.log(app);
            app.controller('treeGridController', function ($scope, $timeout) {
                $scope.tree_data = getTree(myTreeData, 'Id', 'ParentId');
                $scope.my_tree = tree = {};
                $scope.expanding_property = {
                    field: "Name",
                    displayName: "工作区名称",
                    sortable: true,
                    filterable: true,
                    cellTemplate: "<i>{{row.branch[expandingProperty.field]}}</i>"
                };
                $scope.col_defs = [
                    {
                        field: "Description",
                        displayName: "描述"
                    },
                    {
                        field: "Id",
                        displayName: "操作",
                        cellTemplate: "<a type=\"button\" title=\"编辑\" class=\"glyphicon glyphicon-pencil\"  ng-href='/Area/Edit/{{ row.branch[col.field] }}'></a><a style=\"margin-left:10px\" type=\"button\" title=\"删除\" class=\"glyphicon glyphicon-trash\" ng-click='cellTemplateScope.click(row.branch[col.field])'  ></a><a type=\"button\" style=\"margin-left:10px\" title=\"添加子节点\" class=\"glyphicon glyphicon-plus\"  ng-href='/Area/Add/{{ row.branch[col.field]  }}'></a>",
                        cellTemplateScope: {
                            click: function (data) {         // this works too: $scope.someMethod;
                                //console.log(data);
                                layer.confirm('确认删除选中的工作区？', {
                                    btn: ['确认', '取消'] //按钮
                                }, function () {
                                    $.ajax({
                                        type: 'POST',
                                        url: "/Area/Del",
                                        data: { id: data },
                                        dataType: 'json',
                                        success: function (data) {
                                            if (data.State == true) {
                                                layer.msg("删除成功");
                                                location.href = '/Area/Index';
                                            }
                                            else {
                                                layer.msg(data.Message);
                                            }
                                        },
                                        error: function () {
                                            layer.msg('网络错误，请重试！');
                                        }
                                    });
                                }, function () {
                                });
                            }
                        }
                    }
                ];

                $scope.my_tree_handler = function (branch) {
                    console.log('you clicked on', branch)
                }

                $scope.Del = function (code) {
                    alert(code);
                };

                function getTree(data, primaryIdName, parentIdName) {
                    if (!data || data.length == 0 || !primaryIdName || !parentIdName)
                        return [];
                    var tree = [],
                        rootIds = [],
                        item = data[0],
                        primaryKey = item[primaryIdName],
                        treeObjs = {},
                        parentId,
                        parent,
                        len = data.length,
                        i = 0;

                    while (i < len) {

                        item = data[i++];
                        primaryKey = item[primaryIdName];
                        treeObjs[primaryKey] = item;
                        parentId = item[parentIdName];

                        if (parentId) {
                            parent = treeObjs[parentId];

                            if (parent.children) {
                                parent.children.push(item);
                            } else {
                                parent.children = [item];
                            }
                        } else {
                            rootIds.push(primaryKey);
                        }
                    }

                    for (var i = 0; i < rootIds.length; i++) {
                        tree.push(treeObjs[rootIds[i]]);
                    }

                    return tree;
                }
            });

        }).call(this); 
    },
    error: function () {
        layer.msg('网络错误，请重试！');
    }
});

