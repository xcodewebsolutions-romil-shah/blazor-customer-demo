function getLineChart() {

    const ctx = document.getElementById('lineChart');

    new Chart(ctx, {
        type: 'line',
        data: {
            labels: Utils.months({ count: 7 }),
            datasets: [{
                label: 'My First Dataset',
                data: [65, 59, 80, 81, 56, 55, 40],
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
}

function LoadDocument(url) {
    // Initialize PDF.js
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.11.338/pdf.worker.min.js';

    // URL of the PDF file
    const pdfUrl = "http://localhost:5146/" + url;
    //const pdfUrl = 'https://github.com/mozilla/pdf.js/blob/master/test/pdfs/tracemonkey.pdf';
    const prevButton = document.getElementById("prev-page");
    const nextButton = document.getElementById("next-page");
    const zoomInButton = document.getElementById('zoom-in');
    const zoomOutButton = document.getElementById('zoom-out');
    // Fetch the PDF file
    fetch(pdfUrl)
        .then(response => response.arrayBuffer())
        .then(arrayBuffer => {

            var loadingTask = pdfjsLib.getDocument(arrayBuffer);
            loadingTask.promise.then(function (pdfDoc) {
                // you can now use *pdf* here
                // Render PDF pages
                const numPages = pdfDoc.numPages;
                const pdfViewer = document.getElementById('pdf-viewer');
                var pageNumber = 1;
                var scale = 1.5;
                getPageNumber(pageNumber);
                function getPageNumber(pageNum) {
                    if (pageNum == 1)
                        prevButton.style.visibility = "hidden";
                    else
                        prevButton.style.visibility = "visible";

                    if (pageNum == numPages)
                        nextButton.style.visibility = "hidden";
                    else
                        nextButton.style.visibility = "visible";

                    pdfViewer.innerHTML = "";
                    //for (let pageNum = 1; pageNum <= numPages; pageNum++) {
                    pdfDoc.getPage(pageNum).then(page => {
                        const viewport = page.getViewport({ scale });

                        // Create canvas element to render the page
                        const canvas = document.createElement('canvas');
                        const context = canvas.getContext('2d');
                        canvas.height = viewport.height;
                        canvas.width = pdfViewer.parentElement.offsetWidth;
                        //canvas.width = viewport.width;
                        pdfViewer.appendChild(canvas);

                        // Render PDF page to canvas
                        const renderContext = {
                            canvasContext: context,
                            viewport: viewport
                        };
                        page.render(renderContext);
                    });
                }

                prevButton.onclick = function () {
                    pageNumber--;
                    getPageNumber(pageNumber);
                };

                nextButton.onclick = function () {
                    pageNumber++;
                    getPageNumber(pageNumber);
                }

                zoomInButton.onclick = () => {
                    scale += 0.1;
                    getPageNumber(pageNumber);
                };

                zoomOutButton.onclick = () => {
                    if (scale <= 0.2) return;
                    scale -= 0.1;
                    getPageNumber(pageNumber);
                };
            });
        })
        .catch(error => {
            console.error('Error loading PDF:', error);
        });

}

function showToast(isSuccessNew) {
    var toast = new bootstrap.Toast(document.getElementById('liveToast'));
    toast.show();
    if (isSuccessNew) {
        document.getElementById('Data').textContent = "New Solicitation Saved successfully";
    }
    else {
        document.getElementById('Data').textContent = "Changes saved successfully";
    }
}
function showArchiveToast(isSuccessNew) {
    var toast = new bootstrap.Toast(document.getElementById('Toast'));
    toast.show();
    if (isSuccessNew) {
        document.getElementById('DataNew').textContent = "Solicitation archived successfully ";
    }
    else {
        document.getElementById('DataNew').textContent = "Solicitation archiving Failed";
    }
}

function showClosedToast(isSuccessNew) {
    var toast = new bootstrap.Toast(document.getElementById('Toast'));
    toast.show();
    if (isSuccessNew) {
        document.getElementById('DataNew').textContent = "Solicitation closed successfully ";
    }
    else {
        document.getElementById('DataNew').textContent = "Solicitation closing Failed";
    }
}
function SetItem(field, value) {
    localStorage.setItem(field, value);
}

function getItem(id) {
    return localStorage.getItem(id);
}

function toggleSidebar() {
    var sidebarElm = document.getElementById('sidebar');
    var bodyElm = document.getElementById('body-content');
    var utilityLinks = document.getElementById('utility-fixes');
    if (sidebarElm.classList.contains("rz-sidebar-collapsed")) {
        sidebarElm.classList.add("rz-sidebar-expanded");
        bodyElm.classList.add("rz-body-expanded");
        sidebarElm.classList.remove("rz-sidebar-collapsed");
        utilityLinks.style.display = 'flex';
    }
    else {
        bodyElm.classList.remove("rz-body-expanded");
        sidebarElm.classList.add("rz-sidebar-collapsed");
        sidebarElm.classList.remove("rz-sidebar-expanded");
        utilityLinks.style.display = 'none';
    }
}

function restrictToNumbers(element) {
    element.addEventListener('keypress', function (e) {
        if (!/\d/.test(e.key)) {
            e.preventDefault();
        }
    });

    element.addEventListener('input', function (e) {
        this.value = this.value.replace(/\D/g, '');
    });
}
function convertToLocalTimeZone(utcDateTime) {
    const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;
    time = moment.utc(utcDateTime).tz(timeZone).format('YYYY-MM-DD HH:mm:ss');
    return time;
}


function togglePassword() {
    var input = document.getElementById('login-password-text');
    var type = input.getAttribute('type');

    if (type == 'text') {
        input.setAttribute('type', 'password');
    }
    else {
        input.setAttribute('type', 'text');
    }

    var icon = document.getElementsByClassName('rz-button-icon-left')[0];
    if (icon.innerHTML == 'visibility') {
        icon.innerHTML = 'visibility_off';
    }
    else {
        icon.innerHTML = 'visibility';
    }
}
function maxLengthCheck(object) {
    if (object.value.length > 6)
        object.value = object.value.slice(0, 6);
}
function scrollToTop() {
    document.getElementById('body-content').scrollTo(0, 0);
}


window.profileMenuHandler = {
    initialize: function (dotnetHelper) {
        function outsideClickListener(event) {
            const profileMenu = document.querySelector('.rz-navigation-item-link');
            const profileMenuContent = document.querySelector('.profile');

            if (profileMenuContent && !profileMenu.contains(event.target) && !profileMenuContent.contains(event.target)) {
                dotnetHelper.invokeMethodAsync('CloseProfileMenu');
            }
        }

        document.addEventListener('click', outsideClickListener);
    },
    dispose: function () {
        document.removeEventListener('click', outsideClickListener);
    }
};

function showRadarChart(analyses, sow, proposal, delta, sowMatch, proposalMatch, deltaMatch, initial) {


    let radarChartStatus = Chart.getChart("radarChart"); // <canvas> id
    if (radarChartStatus != undefined) {
        radarChartStatus.destroy();
    }

    var sowDataSet = {
        label: 'SOW',
        data: analyses.map(x => x.sowCount),
        fill: true,
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgb(255, 99, 132)',
        pointBackgroundColor: 'rgb(255, 99, 132)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(255, 99, 132)'
    };

    var proposalDataSet = {
        label: 'Proposal',
        data: analyses.map(x => x.proposalCount),
        fill: true,
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        borderColor: 'rgb(54, 162, 235)',
        pointBackgroundColor: 'rgb(54, 162, 235)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(54, 162, 235)'
    };

    var deltaDataSet = {
        label: 'Delta',
        data: analyses.map(x => x.delta),
        fill: true,
        backgroundColor: 'rgba(128, 0, 128, 0.2)',
        borderColor: 'rgb(128, 0, 128)',
        pointBackgroundColor: 'rgb(128, 0, 128)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(128, 0, 128)'
    }    

    const data = {
        labels: analyses.map(x => x.word),
        datasets: []
    };    

    if (sow) {
        data.datasets.push(sowDataSet);
    }

    if (proposal) {
        data.datasets.push(proposalDataSet);
    }

    if (delta) {
        data.datasets.push(deltaDataSet);
    }

    const ctx = document.getElementById('radarChart');
    new Chart(ctx, {
        type: 'radar',
        data: data,
        options: {
            elements: {
                line: {
                    borderWidth: 3
                }
            }
        },
    });
    showWordCloudChart(analyses, 100, "both");
    if (initial === true) {
        renderComparisonBarChart(analyses, sowMatch, proposalMatch, deltaMatch);
    }
}

function showWordCloudChart(analyses, fontSize, fontType) {
    if (fontType == "sow" || fontType == "both") {
        const sowCTX = document.getElementById('wordSOWChart');
        let sowChartStatus = Chart.getChart("wordSOWChart"); // <canvas> id
        if (sowChartStatus != undefined) {
            sowChartStatus.destroy();
        }
        var sowCountMaxValue = Math.max(...analyses.map(a => a.sowCount));
        new Chart(sowCTX.getContext("2d"), {
            type: "wordCloud",
            data: {
                labels: analyses.map(d => d.word),
                datasets: [
                    {
                        label: "",
                        data: analyses.map(d => Math.round((fontSize * d.sowCount) / sowCountMaxValue)),
                        color: ['#0000ff', '#8b0000', '#bdb76b', '#000685', '#008b8b', '#3b8371', '#ff7c75']
                    }
                ]
            },
            options: {
                title: {
                    display: false,
                    text: "Chart.js Word Cloud"
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    wordCloud: {
                        rotate: 0 // Force all words to be horizontal
                    }
                },
                maxRotation: 0,
                minRotation: 0,
            }
        });
    }
    if (fontType == "proposal" || fontType == "both") {
        const proposalCTX = document.getElementById('wordProposalChart');
        let proposalChartStatus = Chart.getChart("wordProposalChart"); // <canvas> id
        if (proposalChartStatus != undefined) {
            proposalChartStatus.destroy();
        }

        var proposalCountMaxValue = Math.max(...analyses.map(a => a.proposalCount));

        new Chart(proposalCTX.getContext("2d"), {
            type: "wordCloud",
            data: {
                labels: analyses.map((d) => d.word),
                datasets: [
                    {
                        label: "",
                        data: analyses.map(d => Math.round((fontSize * d.proposalCount) / proposalCountMaxValue)),
                        color: ['#0000ff', '#8b0000', '#bdb76b', '#000685', '#008b8b', '#3b8371', '#ff7c75']
                    }
                ],
                placementStrategy: 'center'
            },
            options: {
                title: {
                    display: false,
                    text: "Chart.js Word Cloud"
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    wordCloud: {
                        rotate: 0 // Force all words to be horizontal
                    }
                },
                maxRotation: 0,
                minRotation: 0,
            }
        });
    }
}
function renderComparisonBarChart(analyses, sowMatch, proposalMatch, deltaMatch) {

    const comparisonBarCtx = document.getElementById('comparison-bar-chart').getContext('2d');
    let comparisonBarChartStatus = Chart.getChart("comparison-bar-chart");
    if (comparisonBarChartStatus != undefined) {
        comparisonBarChartStatus.destroy();
    }
    analyses.sort((a, b) => a.word.localeCompare(b.word));
    const data = {
        labels: analyses.map(x => x.word),
        datasets: []
    };

    var sowDataSet = {
        label: 'SOW',
        data: analyses.map(x => x.sowCount),
        barThickness: 5,
        barPercentage: 0.8,
        categoryPercentage: 0.8,
        fill: true,
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: '#fff',
        pointBackgroundColor: 'rgb(255, 99, 132)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(255, 99, 132)',
    };

    var proposalDataSet = {
        label: 'Proposal',
        categoryPercentage: 0.8,
        data: analyses.map(x => x.proposalCount),
        fill: true,
        barPercentage: 0.8,
        barThickness: 5,
        backgroundColor: 'rgb(54, 162, 235)',
        borderColor: 'rgb(54, 162, 235)',
        pointBackgroundColor: 'rgb(54, 162, 235)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(54, 162, 235)',
    };

    var deltaDataSet = {
        label: 'Delta',
        categoryPercentage: 0.8,
        data: analyses.map(x => (x.proposalCount - x.sowCount)),
        fill: true,
        barThickness: 5,
        barPercentage: 0.8,
        backgroundColor: '#800080',
        borderColor: 'rgb(201, 203, 207)',
        pointBackgroundColor: 'rgb(201, 203, 207)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(201, 203, 207)',
    }

    if (sowMatch) {
        data.datasets.push(sowDataSet);
    }

    if (proposalMatch) {
        data.datasets.push(proposalDataSet);
    }

    if (deltaMatch) {
        data.datasets.push(deltaDataSet);
    }

    new Chart(comparisonBarCtx, {
        type: 'bar',
        data: data,
        options: {
            maintainAspectRatio: false,
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true
                },
                y: {
                    categoryPercentage: 0.8, // Reduce this value to increase the space between categories
                    barPercentage: 0.5, // Reduce this value to increase the space between bars within categories
                    barThickness: 4, // Explicitly set the thickness of each bar
                    ticks: {
                        autoSkip: false, // Ensure all labels are displayed
                        maxRotation: 0,
                        minRotation: 0
                    }
                }
            },
            ticks: {
                stepSize: 5// Adjust the step size as needed
            }
        }
    });
}

function renderMatchChartForReport(analyses, sowMatch, proposalMatch, deltaMatch, id) {

    const comparisonBarCtx = document.getElementById(id).getContext('2d');
    let comparisonBarChartStatus = Chart.getChart(id);
    if (comparisonBarChartStatus != undefined) {
        comparisonBarChartStatus.destroy();
    }

    const data = {
        labels: analyses.map(x => x.word),
        datasets: []
    };

    var sowDataSet = {
        label: 'SOW',
        data: analyses.map(x => x.sowCount),
        barThickness: 5,
        barPercentage: 0.8,
        categoryPercentage: 0.8,
        fill: true,
        backgroundColor: 'rgba(255, 99, 132)',
        borderColor: '#fff',
        pointBackgroundColor: 'rgb(255, 99, 132)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(255, 99, 132)',
    };

    var proposalDataSet = {
        label: 'Proposal',
        categoryPercentage: 0.8,
        data: analyses.map(x => x.proposalCount),
        fill: true,
        barPercentage: 0.8,
        barThickness: 5,
        backgroundColor: 'rgba(54, 162, 235)',
        borderColor: 'rgb(54, 162, 235)',
        pointBackgroundColor: 'rgb(54, 162, 235)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(54, 162, 235)',
    };

    var deltaDataSet = {
        label: 'Delta',
        categoryPercentage: 0.8,
        data: analyses.map(x => (x.proposalCount - x.sowCount)),
        fill: true,
        barThickness: 5,
        barPercentage: 0.8,
        backgroundColor: '#800080',
        borderColor: 'rgb(201, 203, 207)',
        pointBackgroundColor: 'rgb(201, 203, 207)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(201, 203, 207)',
    }

    if (sowMatch) {
        data.datasets.push(sowDataSet);
    }

    if (proposalMatch) {
        data.datasets.push(proposalDataSet);
    }

    if (deltaMatch) {
        data.datasets.push(deltaDataSet);
    }

    new Chart(comparisonBarCtx, {
        type: 'bar',
        data: data,
        options: {
            maintainAspectRatio: false,
            indexAxis: 'y',
            scales: {
                x: {
                    beginAtZero: true
                },
                y: {
                    categoryPercentage: 0.8, // Reduce this value to increase the space between categories
                    barPercentage: 0.5, // Reduce this value to increase the space between bars within categories
                    barThickness: 4, // Explicitly set the thickness of each bar
                    ticks: {
                        autoSkip: false, // Ensure all labels are displayed
                        maxRotation: 0,
                        minRotation: 0
                    }
                }
            },
            ticks: {
                stepSize: 5 // Adjust the step size as needed
            }
        }
    });
}


function GeneratePDF(analyses, params, show, dotnetObj) {

    var pdfReportElm = document.getElementById('pdf-report-parent');
    pdfReportElm.style.display = 'block';
    pdfReportElm.style.visibility = 'hidden';

    let radarChartStatus = Chart.getChart("report-radar-chart");
    if (radarChartStatus != undefined) {
        radarChartStatus.destroy();
    }


    var sowDataSet = {
        label: ' SOW ',
        data: analyses.map(x => x.sowCount),
        fill: true,
        backgroundColor: 'rgba(255, 99, 132, 0.2)',
        borderColor: 'rgb(255, 99, 132)',
        pointBackgroundColor: 'rgb(255, 99, 132)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(255, 99, 132)'
    };

    var proposalDataSet = {
        label: ' Proposal ',
        data: analyses.map(x => x.proposalCount),
        fill: true,
        backgroundColor: 'rgba(54, 162, 235, 0.2)',
        borderColor: 'rgb(54, 162, 235)',
        pointBackgroundColor: 'rgb(54, 162, 235)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(54, 162, 235)'
    };

    var deltaDataSet = {
        label: ' Delta ',
        data: analyses.map(x => x.delta),
        fill: true,
        backgroundColor: 'rgba(128, 0, 128, 0.2)',
        borderColor: 'rgb(128, 0, 128)',
        pointBackgroundColor: 'rgb(128, 0, 128)',
        pointBorderColor: '#fff',
        pointHoverBackgroundColor: '#fff',
        pointHoverBorderColor: 'rgb(128, 0, 128)'
    }

    const data = {
        labels: analyses.map(x => x.word),
        datasets: []
    };

    if (params.sow) {
        data.datasets.push(sowDataSet);
    }

    if (params.proposal) {
        data.datasets.push(proposalDataSet);
    }

    if (params.delta) {
        data.datasets.push(deltaDataSet);
    }

    if (params.includeRadarInReport) {

        const ctx = document.getElementById('report-radar-chart');
        new Chart(ctx.getContext("2d"), {
            type: 'radar',
            data: data,
            options: {
                elements: {
                    line: {
                        borderWidth: 3
                    }
                }
            },
        });
    }

    const sowCTX = document.getElementById('report-sow-word-chart');
    let sowChartStatus = Chart.getChart("report-sow-word-chart");
    if (sowChartStatus != undefined) {
        sowChartStatus.destroy();
    }
    if (params.includeSOWWordCloudInReport) {
        document.getElementById('word-cloud-chart-div').style.display = 'block';
        var sowCountMaxValue = Math.max(...analyses.map(a => a.sowCount));
        new Chart(sowCTX.getContext("2d"), {
            type: "wordCloud",
            data: {
                labels: analyses.map(d => d.word),
                datasets: [
                    {
                        label: "",
                        data: analyses.map(d => Math.round((params.sowFontSize * d.sowCount) / sowCountMaxValue)),
                        color: ['#0000ff', '#8b0000', '#bdb76b', '#000685', '#008b8b', '#3b8371', '#ff7c75']
                    }
                ]
            },
            options: {
                title: {
                    display: false,
                    text: "Chart.js Word Cloud"
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    wordCloud: {
                        rotate: 0
                    }
                },
                maxRotation: 0,
                minRotation: 0,
            }
        });
    }
    else {
        document.getElementById('page-4').style.display = 'none';
    }

    const proposalCTX = document.getElementById('report-proposal-word-chart');
    let proposalChartStatus = Chart.getChart("report-proposal-word-chart"); // <canvas> id
    if (proposalChartStatus != undefined) {
        proposalChartStatus.destroy();
    }
    if (params.includeProposalWordCloudInReport) {
        document.getElementById('proposal-cloud-chart-div').style.display = 'block';
        var proposalCountMaxValue = Math.max(...analyses.map(a => a.proposalCount));

        new Chart(proposalCTX.getContext("2d"), {
            type: "wordCloud",
            data: {
                labels: analyses.map((d) => d.word),
                datasets: [
                    {
                        label: "",
                        data: analyses.map(d => Math.round((params.proposalFontSize * d.proposalCount) / proposalCountMaxValue)),
                        color: ['#0000ff', '#8b0000', '#bdb76b', '#000685', '#008b8b', '#3b8371', '#ff7c75']
                    }
                ],
                placementStrategy: 'center'
            },
            options: {
                title: {
                    display: false,
                    text: "Chart.js Word Cloud"
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    wordCloud: {
                        rotate: 0
                    }
                },
                maxRotation: 0,
                minRotation: 0,
            }
        });
    }
    else {
        document.getElementById('page-5').style.display = 'none';
    }

    if (params.includeMatchInReport) {
        analyses.sort((a, b) => a.word.localeCompare(b.word));
        renderMatchChartForReport(analyses.slice(0, 24), params.sowMatch, params.proposalMatch, params.deltaMatch, 'report-match-chart-1');
        if (analyses.length > 25) {
            renderMatchChartForReport(analyses.slice(25, 50), params.sowMatch, params.proposalMatch, params.deltaMatch, 'report-match-chart-2');
        }
        else {
            document.getElementById('match-card-2').style.display = 'none';
        }
    }
    else {
        document.getElementById('matchChart').style.display = 'none';
    }

    if (show) {
        setTimeout(() => {
            var element = document.getElementById('pdf-report');

            document.getElementById('pdf-report').style.visibility = 'visible';
            var opt = {
                margin: 0,
                filename: 'myfile.pdf',
                image: { type: 'jpeg', quality: 0.8 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
            };

            html2pdf().set(opt).from(element).output('datauristring').then(function (pdfAsString) {
                document.getElementById('page-4').style.display = 'block';
                document.getElementById('page-5').style.display = 'block';
                document.getElementById('match-card-2').style.display = 'block';
                document.getElementById('matchChart').style.display = 'block';

                var pdfReportElm = document.getElementById('pdf-report-parent');
                pdfReportElm.style.display = 'none';
                //our code
                const newWindow = window.open();
                document.getElementById('pdf-report').style.visibility = 'hidden';
                newWindow.document.write('<iframe width="100%" height="100%" src="' + pdfAsString + '"></iframe>');
                //our code                
            });
        }, 2000);
    }
    else {
        setTimeout(() => {
            var element = document.getElementById('pdf-report');

            document.getElementById('pdf-report').style.visibility = 'visible';
            var opt = {
                margin: 0,
                filename: 'myfile.pdf',
                image: { type: 'jpeg', quality: 0.98 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
            };
            html2pdf().set(opt).from(element).output('datauristring').then(function (pdfAsString) {
                document.getElementById('pdf-report').style.visibility = 'hidden';
                var fileBytes = base64ToUint8Array(pdfAsString.split(',')[1]);
                sendChunksToDotNet(fileBytes, dotnetObj, 256 * 1024);

                document.getElementById('page-4').style.display = 'block';
                document.getElementById('page-5').style.display = 'block';
                document.getElementById('matchChart').style.display = 'block';
                document.getElementById('match-card-2').style.display = 'block';
            });
        }, 2000);
    }
}

// Function to convert base64 to Blob
function base64ToBlob(base64, mime) {
  
}


async function sendChunksToDotNet(data, dotnetObj, chunkSize = 256 * 1024) {
    for (let i = 0; i < data.length; i += chunkSize) {
        const chunk = data.slice(i, i + chunkSize);
        await dotnetObj.invokeMethodAsync('SaveFileChunk', chunk);
    }
    dotnetObj.invokeMethodAsync('SaveFileBase64String');

    var pdfReportElm = document.getElementById('pdf-report-parent');
    //pdfReportElm.style.display = 'none';
}

function base64ToUint8Array(base64) {
    // Decode the Base64 string to a binary string
    let binaryString = atob(base64);

    // Create a Uint8Array with the same length as the binary string
    let len = binaryString.length;
    let bytes = new Uint8Array(len);

    // Convert the binary string to a Uint8Array
    for (let i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }

    return bytes;
}