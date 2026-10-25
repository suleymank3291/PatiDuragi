// Besleme noktalarını OpenStreetMap haritası üzerinde işaretler (Leaflet kütüphanesi).
function haritaCiz(noktalar) {
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
}

// Veriyi HTML olarak değil düz metin olarak basmak için
function metin(deger) {
    const div = document.createElement("div");
    div.textContent = deger;
    return div.innerHTML;
}
