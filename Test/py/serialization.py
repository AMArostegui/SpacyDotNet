import spacy

from spacy.tokens import DocBin
from spacy.tokens import Doc
from spacy.vocab import Vocab

def print_doc(adoc):
    for word in adoc:
        lexeme = adoc.vocab[word.text]
        print(lexeme.text, lexeme.orth, lexeme.shape_, lexeme.prefix_, lexeme.suffix_,
              lexeme.is_alpha, lexeme.is_digit, lexeme.is_title, lexeme.lang_)

text = "I love coffee"

# Load base document
nlp = spacy.load("en_core_web_sm")
doc_base = nlp(text)
print("")
print_doc(doc_base)

# Serialize document to disk and bytes
doc_base.to_disk("doc.spacy")
doc_base_bytes = doc_base.to_bytes()

# Serialize using DocBin
docbin_base = DocBin(attrs=["ENT_IOB", "POS", "HEAD", "DEP", "ENT_TYPE"], store_user_data=True)
docbin_base.add(doc_base)
docbin_base_bytes = docbin_base.to_bytes()

# Restore document from disk
doc = Doc(Vocab())
doc.from_disk("doc.spacy")
print("")
print_doc(doc)

# Restore document from bytes
doc = Doc(Vocab())
doc.from_bytes(doc_base_bytes)
print("")
print_doc(doc)

# Restore using DocBin
docbin = DocBin().from_bytes(docbin_base_bytes)
docs = list(docbin.get_docs(nlp.vocab))
print("")
print_doc(docs[0])