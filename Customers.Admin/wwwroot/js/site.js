window.docxInterop = {
    convertToHtml: async function (arrayBuffer, filePath) {
        try {
            //const jsArray = new Uint8Array(arrayBuffer);
            var options = {
                styleMap: [
                    "comment-reference => sup"
                ]
            };
            //const result = await mammoth.convertToHtml({ arrayBuffer: jsArray.buffer});
            const result = await mammoth.convertToHtml({ path: filePath }, options);
            document.getElementById('doc-viewer').innerHTML = result.value;
            return "success";
        } catch (err) {
            console.error("Conversion error:", err);
            return "Error converting file";
        }
    }
};

function detectDevice() {
    var isMobile = /Mobi|Android/i.test(navigator.userAgent);
    if (isMobile) {
        console.log('mobile detected');
        var sidebarElm = document.getElementById('sidebar');
        var bodyElm = document.getElementById('body-content');
        bodyElm.classList.remove("rz-body-expanded");
        sidebarElm.classList.add("rz-sidebar-collapsed");
        sidebarElm.classList.remove("rz-sidebar-expanded");
    }
}

            // wwwroot/tooltipHelper.js
window.tooltipHelper = {
    addHoverEvent: function (elementId, dotNetObjectReference) {
        var element = document.getElementById(elementId);
        if (element) {
            element.addEventListener('mouseenter', function () {
                dotNetObjectReference.invokeMethodAsync('ShowTooltip', elementId);
            });
            element.addEventListener('mouseleave', function () {
                dotNetObjectReference.invokeMethodAsync('HideTooltip');
            });
        }
    }
};