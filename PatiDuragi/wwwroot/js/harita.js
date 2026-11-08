// Besleme noktalarını OpenStreetMap haritası üzerinde işaretler (Leaflet kütüphanesi).
// sinirlar verilirse (mahalle sınırları, GeoJSON metni) bunlar da ince kırmızı çizgiyle çizilir.
function haritaCiz(noktalar, sinirlar = []) {
    const harita = L.map("harita");
    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap katkıcıları"
    }).addTo(harita);

    if (noktalar.length === 0) {
        harita.setView([39.92, 32.85], 11);   // nokta yoksa Ankara merkezi
        return;
    }

    const konumlar = noktalar.map(n => {
        L.marker([n.enlem, n.boylam]).addTo(harita).bindPopup(
            `<strong>${metin(n.ad)}</strong><br>${metin(n.tur)}<br><a href="/Noktalar/Detay/${n.id}">Detaya git</a>`);
        return [n.enlem, n.boylam];
    });

    harita.fitBounds(konumlar, { padding: [40, 40], maxZoom: 16 });

    // Sınırlar, harita konumlandıktan sonra eklenir (Leaflet bu sırayı ister)
    sinirlar.forEach(sinir => L.geoJSON(JSON.parse(sinir), {
        style: { color: "#dc3545", weight: 1.5, fillOpacity: 0.04, dashArray: "4 3" }
    }).addTo(harita));
}

// Bir mahallenin sınırını kırmızı çizgiyle, içindeki besleme noktalarını da işaretle çizer.
// kucuk = true: kart üstündeki önizleme haritası (kaydırma ve yakınlaştırma kapalı).
function bolgeHaritasiCiz(kutuId, sinir, noktalar, kucuk) {
    const harita = L.map(kutuId, {
        zoomControl: !kucuk, dragging: !kucuk, scrollWheelZoom: !kucuk,
        doubleClickZoom: !kucuk, touchZoom: !kucuk, boxZoom: !kucuk, keyboard: !kucuk
    });
    harita.attributionControl.setPrefix(false);
    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap"
    }).addTo(harita);

    // Önce harita sınıra göre konumlanır, çizgi ondan sonra eklenir (Leaflet bu sırayı ister)
    const cizgi = L.geoJSON(JSON.parse(sinir), {
        style: { color: "#dc3545", weight: 2, fillColor: "#dc3545", fillOpacity: 0.06 }
    });
    harita.fitBounds(cizgi.getBounds(), { padding: [10, 10] });
    cizgi.addTo(harita);

    noktalar.forEach(n => {
        if (kucuk) {
            L.circleMarker([n.enlem, n.boylam], { radius: 4, color: "#198754", fillOpacity: 1 }).addTo(harita);
        } else {
            L.marker([n.enlem, n.boylam]).addTo(harita).bindPopup(
                `<strong>${metin(n.ad)}</strong><br>${metin(n.tur)}<br><a href="/Noktalar/Detay/${n.id}">Detaya git</a>`);
        }
    });
}

// Veriyi HTML olarak değil düz metin olarak basmak için
function metin(deger) {
    const div = document.createElement("div");
    div.textContent = deger;
    return div.innerHTML;
}
