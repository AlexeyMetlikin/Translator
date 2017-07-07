class API {
    
    constructor(host, key) {
        
        this.Host = host;   // адрес сервера
        this.Langs = '';    // список языков
        this.Key = key;
    }

    // запрос на сервер request с параметрами requestPars
    // в случае успешного запроса вызвать функцию func
    sendRequst(request, requestPars, func) {
        
        var req = this.getXMLHttp();    //получаем новый эземпляр запроса
        
        // вешаем обработчик на изменение статуса запроса
        req.onreadystatechange = function() {  
            if (req.readyState == 4) { // если запрос выполнен
                if(req.status == 200) { // и без ошибок
                    func(JSON.parse(req.responseText)); //вызываем func с результатом запроса
                }
                else {                
                    alert('Ошибка запроса на сервер: '+ req.responseText);  //выводим сообщение об ошибке
                }
            }
        }
        
        req.open('POST', this.Host + request, true);    // инициализируем тип и путь запроса
        req.setRequestHeader('Content-type', 'application/x-www-form-urlencoded');
        req.send(prepareParams(requestPars));   // отправляем запрос с параметрами
    }
    
    getXMLHttp () {
        var q;
        try {
            q=new ActiveXObject("Msxml2.XMLHTTP");          // IE
        } 
        catch(error1) {
            try {
                q=new ActiveXObject("Microsoft.XMLHTTP");   // IE
            } 
            catch (error2) {
                q=false;
            }
        }
        if (!q) {
            q = new XMLHttpRequest();   // остальные браузеры
        }
        return q;
    }
    
}

// новый экземпляр API;
translateAPI = new API
(
    'https://translate.yandex.net',
    'trnsl.1.1.20170705T051451Z.be2ac132d81993cd.d05ddc65d1cb87498519a919b46278f54265b15c'
); 

// Функция преобразования параметров запроса в строку с разделением символом &
function prepareParams(requestPars) {
     var strParams = '';
     for(i in requestPars) {
         strParams += i + '=' + requestPars[i] + '&';
     }
     return strParams.slice(0, -1); // возвращаем результат без последнего символа (&)
}

// Заполнеие панели со списком языков
// Результат представляет из себя n колонок (div-ов) из 15 языков в одной колонке
function fillLanguagesPanel(data) {
    
    translateAPI.Langs = data.langs; //Заполняем список языков в API
    
    var cols = 15;  // количество языков в одной колонке
    
    var elem = document.getElementById('languages-panel');
    var languages = '<div class="lang-list-column">';
    var i = 1;
    
    for(var lang in data.langs) {
        
        if(i > cols) {
            languages += '</div>';
            i = 1;
            languages += '<div class="lang-list-column">';
        }  
                
        languages += '<div class="lang-item" data-lang-name=' + 
                          lang + '>' +
                          data.langs[lang] + '</div>';
        i++;
    }
    
    if(i != 1) {
        languages += '</div>';
    }
    elem.innerHTML = languages;     // заполняем панель полученным списком
    
    fillCurrentLangs(data.langs);   // выставляем языки по умолчанию
}

// функция выставления языков по умолчанию
function fillCurrentLangs(langs) {
    var langTo = document.getElementById('selected-language-to');       // язык, на который переводим
    var langFrom = document.getElementById('selected-language-from');   // язык, с которого переводим
    
    // если в списке языков есть русский - устанавливаем в качестве языка по умолчанию, на который переводим
    if(translateAPI.Langs['ru']) {              
        langTo.innerHTML = translateAPI.Langs['ru'];
        langTo.setAttribute('data-lang-name', 'ru');
    }
    else {
        for(lang in translateAPI.Langs) {    // иначе выбираем первый из списка языков 
            langTo.innerHTML = langs[lang];
            langTo.setAttribute('data-lang-name', lang);
            break;
        }
    }
    
    // если в списке языков есть русский - устанавливаем в качестве языка по умолчанию, с которого переводим
    if(translateAPI.Langs['en']) {
        langFrom.innerHTML = translateAPI.Langs['en'];
        langFrom.setAttribute('data-lang-name', 'en');
    }
    else {
        for(lang in translateAPI.Langs) {    // иначе выбираем первый из списка языков 
            langFrom.innerHTML = langs[lang];
            langFrom.setAttribute('data-lang-name', lang);
            break;
        }
    }
}

// показать панель выбора языка
// selected - поле, для которого выбираем язык
function showLanguagesPanel(selected, event) {
    
    event.stopPropagation();    // остановить 'всплытие'
    selected.classList.add("is-selected");  // выделить поле, для которого выбираем язык

    var panel = document.getElementById('languages-panel'); // получаем панель со списком
    
    panel.setAttribute('tabindex','-1');    // чтобы программно выставить фокус, tabindex=-1
    panel.style.display = 'block';          // отображаем панель со списком
    panel.style.top = selected.offsetHeight + selected.offsetTop + 3;
    panel.style.left = document.getElementsByClassName('content__body')[0].offsetLeft - 25;
    
    panel.focus();    // выставляем на ней фокус

    panel.onblur = function(e) {    // обработчик потери фокуса панели
        hideLanguagePanel(panel, selected); // скрываем панель при потере фокуса
    };
    
    panel.onclick = function(e) {   // обработчик клика по панели
        chooseLanguage(e.target, panel, selected);  // выбираем язык
    }
}

// скрыть панель выбора языка
function hideLanguagePanel(panel, selected){
    panel.style.display = 'none';               // скрыть панель выбора языка 
    selected.classList.remove('is-selected');   // удаляем у поля выделение
    panel.onblur = null;                        // снимаем обработчик потери фокуса панели
}

// выбираем язык
function chooseLanguage(elem, panel, selected){
    if(elem.classList.contains('lang-item')){   // если выбран какой-то язык
        selected.setAttribute('data-lang-name', elem.getAttribute('data-lang-name'));
        selected.innerHTML = elem.innerHTML;
        
        hideLanguagePanel(panel, selected); // скрываем панель
        panel.onclick = null;               // снимаем обработчик клика по панели
    }   
}

// функция смены местами языков
function reverseLanguages() {
    var langTo = document.getElementById('selected-language-to');       // язык, на который переводим
    var langFrom = document.getElementById('selected-language-from');   // язык, с которого переводим
    
    var tempLang = langTo.innerHTML;
    var tempLangName = langTo.getAttribute('data-lang-name');
    
    langTo.innerHTML = langFrom.innerHTML;
    langTo.setAttribute('data-lang-name', langFrom.getAttribute('data-lang-name'));
    
    langFrom.innerHTML = tempLang;
    langFrom.setAttribute('data-lang-name', tempLangName);
}

// запуск функции Function_prototype, если прошел интервал времени delay 
(function(Function_prototype) {
    Function_prototype.debounce = function(delay, ctx) {
        var fn = this, timer;
        return function() {
            var args = arguments, that = this;
            clearTimeout(timer);                // при каждом вызове сбрасываем таймер
            timer = setTimeout(function() {     // запускаем новый таймер с интервалом delay
                fn.apply(ctx || that, args); 
            }, delay);
        };
    };
})(Function.prototype);

// функция автоопределения языка с учетом уже выбранного
function tryDetectLanguage() {
    var text = document.getElementById('initial-text').value;   // текст, язык которого необходимо определить
    if(text) {  // если текстовое поле не пусто
        
        // выбранный язык, с которого переводим
        var langFrom = document.getElementById('selected-language-from').getAttribute('data-lang-name');
        if(!langFrom){
            langFrom = 'en';    // если не заполнен - английский по умолчанию
        }      
        
        var requestParams = 
            {
                hint : langFrom,        // список вероятных языков текста 
                text : text,            // исходный текст
                key : translateAPI.Key  // ключ API
            };
        var response = translateAPI.sendRequst('/api/v1.5/tr.json/detect', requestParams, fillInitialTextLang); // выполняем запрос
    }
    else {
        document.getElementById('translating-text').value = '';
    }
}

// функция заполнения автоматически определенного языка (результат tryDetectLanguage)
function fillInitialTextLang(response) {
    var langFrom = document.getElementById('selected-language-from');
    langFrom.innerHTML = translateAPI.Langs[response.lang];
    langFrom.setAttribute('data-lang-name', response.lang);
}

// функция получения списка доступных язков
function initLanguages() {
    var requestParams = 
        {
            ui : 'ru',                  // на каком языке получать список 
            key : translateAPI.Key      // ключ API
        };
    var response = translateAPI.sendRequst('/api/v1.5/tr.json/getLangs', requestParams, fillLanguagesPanel);
}

// функция перевода текста
function translateText() {
    var text = document.getElementById('initial-text').value;           // текст для перевода
    if(text) {  // если текстовое поле не пусто
        var langTo = document.getElementById('selected-language-to');       // язык, на который переводим
        var langFrom = document.getElementById('selected-language-from');   // язык, с которого переводим
    
        var requestParams = 
            {
                lang : langFrom.getAttribute('data-lang-name') + '-' + langTo.getAttribute('data-lang-name'),         // язык (с какого на какой переводить, например 'en-ru')
                option : 1,     // определяет правильность указания исходного языка в параметре lang
                text : text,    // исходный текст
                key : translateAPI.Key  // ключ API
            };
        var response = translateAPI.sendRequst('/api/v1.5/tr.json/translate', requestParams, getTranslateResult);   // выполняем запрос
    }
}

// функция вывода результата перевода
function getTranslateResult(result){
    document.getElementById('translating-text').value = result.text;
}

function clearInintialText() {
    document.getElementById('initial-text').value = '';
    document.getElementById('translating-text').value = '';
}

// выполнить при полной загрузке страницы
window.onload=function(){
    
    initLanguages();    // получаем список доступных язков
    
    var selected = document.getElementsByClassName('select-language');
    for(i = 0; i < selected.length; i++) {
        selected[i].onclick = function (e) {    // вешаем обработчик нажатия на поле выбора языка
            showLanguagesPanel(this, e);        // показать панель выбора языка
            return false;
        }
    }
    
    var translateButton = document.getElementById('translate-button');
    translateButton.onclick = function (e) {    // обработчик клика на кнопку "Перевести"
        translateText();
        return false;
    };
    
    var reverseButton = document.getElementById('reverse-languages');
    reverseButton.onclick = function (e) {      // Обработчик нажатия на кнопку смены языков
        reverseLanguages();
        return false;
    };
    
    var initialText = document.getElementById('initial-text');
    // обработчик на окончание ввода текста (интервал 1 сек.)
    initialText.oninput = tryDetectLanguage.debounce(1000);
    
    var clearText = document.getElementById('clear-initial-text');
    clearText.onclick = function (e) {      // Обработчик нажатия на кнопку очистки текста
        clearInintialText();
        return false;
    };
}