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

// Nokta formundaki küçük harita: tıklanan yere işaret konur, Enlem ve Boylam kutuları dolar.
// Kayıtlı mahallelerin sınırları çizilir ve seçilen konumun hangi mahalleye düştüğü yazılır.
//   kutuId      : harita kutusunun id'si (data-enlem / data-boylam: koordinat kutularının id'leri)
//   bolgeler    : [{ id, mahalle, ilce, sinir }]  (sinir = GeoJSON metni)
//   odakBolgeId : "Bu bölgeye nokta ekle" ile gelindiyse o mahalle; harita oraya odaklanır
function konumSecici(kutuId, bolgeler, odakBolgeId) {
    const kutu = document.getElementById(kutuId);
    const enlemKutusu = document.getElementById(kutu.dataset.enlem);
    const boylamKutusu = document.getElementById(kutu.dataset.boylam);
    const bilgi = document.getElementById(kutuId + "-bilgi");

    // zoomSnap 0.5: mahalle haritaya yarım kademe hassasiyetle sığdırılır (daha yakın görünür)
    const harita = L.map(kutuId, { zoomSnap: 0.5 });
    L.tileLayer("https://tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap katkıcıları"
    }).addTo(harita);

    // Sınır katmanları önce oluşturulur, harita konumlandıktan sonra eklenir (Leaflet bu sırayı ister)
    const sinirlar = bolgeler.map(b => ({
        bolge: b,
        sekil: JSON.parse(b.sinir),
        katman: L.geoJSON(JSON.parse(b.sinir), {
            style: { color: "#dc3545", weight: b.id === odakBolgeId ? 2.5 : 1.5,
                     fillColor: "#dc3545", fillOpacity: b.id === odakBolgeId ? 0.08 : 0.03 }
        }).bindTooltip(`${metin(b.mahalle)} · ${metin(b.ilce)}`, { sticky: true })
    }));

    let isaret = null;

    // Kutulardaki değerlerden konumu okur; geçerli sayı değilse null döner
    function kutulardanKonum() {
        const enlem = parseFloat(enlemKutusu.value);
        const boylam = parseFloat(boylamKutusu.value);
        return isNaN(enlem) || isNaN(boylam) ? null : [enlem, boylam];
    }

    // İşareti konuma taşır ve konumun hangi mahallede olduğunu yazar
    function isaretiGoster(konum, ortala) {
        if (isaret === null) {
            isaret = L.marker(konum, { draggable: true }).addTo(harita);
            isaret.on("dragend", () => kutulariDoldur(isaret.getLatLng()));
        } else {
            isaret.setLatLng(konum);
        }
        if (ortala) {
            harita.setView(konum, Math.max(harita.getZoom() || 0, 16));
        }

        const bulunan = sinirlar.find(s => icindeMi(konum[0], konum[1], s.sekil));
        if (bulunan) {
            bilgi.className = "small mt-1 text-success";
            bilgi.innerHTML = `<i class="bi bi-check-circle"></i> Bu konum <strong>${metin(bulunan.bolge.mahalle)}</strong>
                (${metin(bulunan.bolge.ilce)}) sınırları içinde; nokta bu mahalleye atanacak.`;
        } else {
            bilgi.className = "small mt-1 text-danger";
            bilgi.innerHTML = `<i class="bi bi-exclamation-triangle"></i> Bu konum kayıtlı hiçbir mahallenin içinde değil.
                Kırmızı çizgili alanlardan birinin içine tıklayın.`;
        }
    }

    // Haritadan seçilen konumu kutulara yazar ve kutuların doğrulamasını tazeler
    function kutulariDoldur(latlng) {
        enlemKutusu.value = latlng.lat.toFixed(6);
        boylamKutusu.value = latlng.lng.toFixed(6);
        if (window.jQuery && jQuery.fn.valid) {
            jQuery(enlemKutusu).valid();
            jQuery(boylamKutusu).valid();
        }
        isaretiGoster([latlng.lat, latlng.lng], false);
    }

    // İlk görünüm: kutularda konum varsa orası, yoksa hedef mahalle, yoksa tüm mahalleler
    const ilkKonum = kutulardanKonum();
    const odak = sinirlar.find(s => s.bolge.id === odakBolgeId);
    if (ilkKonum) {
        harita.setView(ilkKonum, 16);
    } else if (odak) {
        harita.fitBounds(odak.katman.getBounds(), { padding: [10, 10] });
    } else if (sinirlar.length > 0) {
        const tumu = L.featureGroup(sinirlar.map(s => s.katman));
        harita.fitBounds(tumu.getBounds(), { padding: [20, 20] });
    } else {
        harita.setView([39.92, 32.85], 11);   // Ankara merkezi
    }

    sinirlar.forEach(s => s.katman.addTo(harita));
    if (ilkKonum) {
        isaretiGoster(ilkKonum, false);
    }

    harita.on("click", e => kutulariDoldur(e.latlng));

    // Koordinat elle yazılırsa işaret de oraya gider
    [enlemKutusu, boylamKutusu].forEach(k => k.addEventListener("change", () => {
        const konum = kutulardanKonum();
        if (konum) {
            isaretiGoster(konum, true);
        }
    }));
}

// Konum mahalle sınırının içinde mi? Sunucudaki SinirServisi.IcindeMi ile aynı ışın yöntemi.
function icindeMi(enlem, boylam, sekil) {
    const cokgenler = sekil.type === "MultiPolygon" ? sekil.coordinates : [sekil.coordinates];
    let icinde = false;
    cokgenler.forEach(cokgen => cokgen.forEach(halka => {
        // GeoJSON'da her köşe [boylam, enlem] sırasıyla yazılır
        for (let i = 0, j = halka.length - 1; i < halka.length; j = i++) {
            const [bi, ei] = halka[i];
            const [bj, ej] = halka[j];
            if ((ei > enlem) !== (ej > enlem) && boylam < (bj - bi) * (enlem - ei) / (ej - ei) + bi) {
                icinde = !icinde;
            }
        }
    }));
    return icinde;
}

// Veriyi HTML olarak değil düz metin olarak basmak için
function metin(deger) {
    const div = document.createElement("div");
    div.textContent = deger;
    return div.innerHTML;
}
