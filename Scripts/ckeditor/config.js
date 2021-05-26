/**
 * @license Copyright (c) 2003-2014, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function(config) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
    //config.uiColor = '#EFFEF3';
    config.extraPlugins = 'ckeditor_wiris';
    config.enterMode = CKEDITOR.ENTER_BR;
    config.height = 100;
    //config.width = 500;
    config.font_defaultLabel = 'Arial';
    config.fontSize_defaultLabel = '12px';
    config.toolbar=
    [
        ['Image','youtube','html5video', 'Save', 'Maximize','Print', 'Undo', 'Templates', 'Table', 'SpecialChar',
            'SelectAll', 'Preview', 'PasteText', 'Language', 'HorizontalRule', 'Font', 'ckeditor_wiris_formulaEditor','ckeditor_wiris_formulaEditorChemistry', 'Find']
    ];
};

