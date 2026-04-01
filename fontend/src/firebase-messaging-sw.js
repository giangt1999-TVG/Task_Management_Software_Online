importScripts('https://www.gstatic.com/firebasejs/7.21.0/firebase-app.js');
importScripts('https://www.gstatic.com/firebasejs/7.21.0/firebase-messaging.js');

firebase.initializeApp({
    apiKey: "AIzaSyBbGDzxJ6ciX2czz3mGUHkICos8G2sln3s",
    authDomain: "task-management-system-313718.firebaseapp.com",
    projectId: "task-management-system-313718",
    storageBucket: "task-management-system-313718.appspot.com",
    messagingSenderId: "646993374856",
    appId: "1:646993374856:web:e5a3d9e6e6ae9c97feb93d",
    measurementId: "G-J8ECYMNRLR"
});

const messaging = firebase.messaging();