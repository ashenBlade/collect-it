// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const tabsBtn = $('.tabs-nav-btn').toArray();
const tabs = $('.tab').toArray();

tabsBtn.forEach(function (item){
    item.addEventListener('click', function (){
        let currentBtn = item;
        let tabId = currentBtn.getAttribute('data-tab');
        
        tabs.forEach(function (item){
            item.classList.remove('active');
        });
        tabsBtn.forEach(function (item){
           item.classList.remove('active'); 
        });
        
        currentBtn.classList.add('active');
        $(tabId).addClass('active');
        $('.menu').toggleClass('menu-active');
        $('.content').toggleClass('content-active');
        $('.menu-btn').toggleClass('triangle-right').toggleClass('triangle-left');
    });
});
// Write your JavaScript code.
